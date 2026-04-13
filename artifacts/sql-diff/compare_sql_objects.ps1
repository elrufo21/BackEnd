function Get-ObjectInventory([string]$path){
  $lines = Get-Content $path
  $items = New-Object System.Collections.Generic.List[object]
  $regex = '^(?<indent>\s*)(?<cmd>CREATE|ALTER)\s+(?<type>PROCEDURE|PROC|FUNCTION|VIEW|TRIGGER|TABLE)\s+(?<name>.+)$'
  for($i=0;$i -lt $lines.Count;$i++){
    $line = $lines[$i].Trim()
    if($line -match $regex){
      $cmd = $Matches['cmd'].ToUpperInvariant()
      $type = $Matches['type'].ToUpperInvariant()
      if($type -eq 'PROC'){ $type = 'PROCEDURE' }
      $nameRaw = $Matches['name']
      $name = $nameRaw
      if($name -match '^(.*?)(\s+AS\b|\s*\()'){ $name = $Matches[1] }
      $name = ($name -replace '--.*$','').Trim()
      $name = ($name -replace '\s+',' ')
      $firstTokens = $name.Split(' ',[System.StringSplitOptions]::RemoveEmptyEntries)
      if($firstTokens.Count -gt 0){ $name = $firstTokens[0] }
      $items.Add([pscustomobject]@{Type=$type;Name=$name;Cmd=$cmd;Line=($i+1)})
    }
  }
  return $items
}

function Get-ObjectBlock([string]$path,[string]$target){
  $lines = Get-Content $path
  $startRegex = '^(?<cmd>CREATE|ALTER)\s+(?<type>PROCEDURE|PROC|FUNCTION|VIEW|TRIGGER|TABLE)\s+(?<name>.+)$'
  for($i=0;$i -lt $lines.Count;$i++){
    $line=$lines[$i].Trim()
    if($line -match $startRegex){
      $type=$Matches['type'].ToUpperInvariant(); if($type -eq 'PROC'){$type='PROCEDURE'}
      $name=$Matches['name']
      if($name -match '^(.*?)(\s+AS\b|\s*\()'){ $name=$Matches[1] }
      $name=($name -replace '--.*$','').Trim(); $name=($name -replace '\s+',' ')
      $tokens=$name.Split(' ',[System.StringSplitOptions]::RemoveEmptyEntries)
      if($tokens.Count -gt 0){$name=$tokens[0]}
      $key="$type|$name"
      if($key -eq $target){
        $end = $lines.Count - 1
        for($j=$i+1; $j -lt $lines.Count; $j++){
          $t = $lines[$j].Trim()
          if($t -match '^GO$') { $end = $j; break }
          if($t -match $startRegex) { $end = $j-1; break }
        }
        return [string]::Join("`n", $lines[$i..$end])
      }
    }
  }
  return $null
}

$localPath='CLASES_EF/bdlocal.sql'
$actualPath='CLASES_EF/bdactual.sql'
$local = Get-ObjectInventory $localPath
$actual = Get-ObjectInventory $actualPath

$localKeys = $local | ForEach-Object {"$($_.Type)|$($_.Name)"} | Sort-Object -Unique
$actualKeys = $actual | ForEach-Object {"$($_.Type)|$($_.Name)"} | Sort-Object -Unique

$missingInActual = Compare-Object -ReferenceObject $localKeys -DifferenceObject $actualKeys -PassThru | Where-Object {$_ -in $localKeys}
$extraInActual = Compare-Object -ReferenceObject $localKeys -DifferenceObject $actualKeys -PassThru | Where-Object {$_ -in $actualKeys}

Write-Output "MISSING_IN_BDACTUAL_COUNT=$($missingInActual.Count)"
$missingInActual | ForEach-Object { Write-Output "MISSING|$_" }
Write-Output "EXTRA_IN_BDACTUAL_COUNT=$($extraInActual.Count)"
$extraInActual | ForEach-Object { Write-Output "EXTRA|$_" }

$changed = @()
foreach($k in $localKeys){
  if(-not ($actualKeys -contains $k)){ continue }
  $lb = Get-ObjectBlock $localPath $k
  $ab = Get-ObjectBlock $actualPath $k
  if($null -eq $lb -or $null -eq $ab){ continue }
  $ln = ($lb.ToLowerInvariant() -replace '\s+',' ')
  $an = ($ab.ToLowerInvariant() -replace '\s+',' ')
  if($ln -ne $an){ $changed += $k }
}

Write-Output "CHANGED_DEFINITIONS_COUNT=$($changed.Count)"
$changed | ForEach-Object { Write-Output "CHANGED|$_" }

foreach($k in $changed){
  Write-Output "===== DIFF $k ====="
  $lb = Get-ObjectBlock $localPath $k
  $ab = Get-ObjectBlock $actualPath $k
  $alines = $ab -split "`n"
  $llines = $lb -split "`n"
  $d = Compare-Object -ReferenceObject $alines -DifferenceObject $llines -SyncWindow 4
  $d | Select-Object -First 120 | ForEach-Object { Write-Output ("{0} {1}" -f $_.SideIndicator, $_.InputObject) }
}
