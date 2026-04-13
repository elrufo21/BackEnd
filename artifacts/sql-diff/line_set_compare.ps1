function Get-ObjectBlockLines([string]$path,[string]$target){
  $lines = Get-Content $path
  $startRegex = '^(?<cmd>CREATE|ALTER)\s+(?<type>PROCEDURE|PROC|FUNCTION|VIEW|TRIGGER|TABLE)\s+(?<name>.+)$'
  for($i=0;$i -lt $lines.Count;$i++){
    $line=$lines[$i].Trim()
    if($line -match $startRegex){
      $type=$Matches['type'].ToUpperInvariant(); if($type -eq 'PROC'){$type='PROCEDURE'}
      $name=$Matches['name']; if($name -match '^(.*?)(\s+AS\b|\s*\()'){ $name=$Matches[1] }
      $name=($name -replace '--.*$','').Trim(); $name=($name -replace '\s+',' ')
      $tokens=$name.Split(' ',[System.StringSplitOptions]::RemoveEmptyEntries); if($tokens.Count -gt 0){$name=$tokens[0]}
      $key="$type|$name"
      if($key -eq $target){
        $end = $lines.Count - 1
        for($j=$i+1; $j -lt $lines.Count; $j++){
          $t = $lines[$j].Trim()
          if($t -match '^GO$') { $end = $j; break }
          if($t -match $startRegex) { $end = $j-1; break }
        }
        return $lines[$i..$end]
      }
    }
  }
  return @()
}

function NormalizeLine([string]$line){
  if($null -eq $line){ return '' }
  $x = $line
  $x = [regex]::Replace($x,'--.*$','')
  $x = $x.ToLowerInvariant().Trim()
  $x = [regex]::Replace($x,'\s+','')
  return $x
}

$targets=@('PROCEDURE|[dbo].[uspEliminarProducto]','PROCEDURE|[dbo].[uspinsertarNotaB]','PROCEDURE|[dbo].[uspListaBajas]')
foreach($t in $targets){
  "===== $t ====="
  $a=Get-ObjectBlockLines 'CLASES_EF/bdactual.sql' $t
  $l=Get-ObjectBlockLines 'CLASES_EF/bdlocal.sql' $t
  $na = @{}; foreach($line in $a){ $n=NormalizeLine $line; if($n -and -not $na.ContainsKey($n)){ $na[$n]=$line.Trim() } }
  $nl = @{}; foreach($line in $l){ $n=NormalizeLine $line; if($n -and -not $nl.ContainsKey($n)){ $nl[$n]=$line.Trim() } }

  $onlyActual = Compare-Object -ReferenceObject $nl.Keys -DifferenceObject $na.Keys -PassThru | Where-Object {$_ -in $na.Keys}
  $onlyLocal = Compare-Object -ReferenceObject $nl.Keys -DifferenceObject $na.Keys -PassThru | Where-Object {$_ -in $nl.Keys}

  "ONLY_IN_BDACTUAL_COUNT=$($onlyActual.Count)"
  $onlyActual | Select-Object -First 60 | ForEach-Object { "ACTUAL_ONLY: $($na[$_])" }
  "ONLY_IN_BDLOCAL_COUNT=$($onlyLocal.Count)"
  $onlyLocal | Select-Object -First 60 | ForEach-Object { "LOCAL_ONLY: $($nl[$_])" }
}
