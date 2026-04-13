param([string]$Source,[string]$Target,[string]$Out)
$lines=Get-Content $Source
$startRegex='^(?<cmd>CREATE|ALTER)\s+(?<type>PROCEDURE|PROC|FUNCTION|VIEW|TRIGGER|TABLE)\s+(?<name>.+)$'
for($i=0;$i -lt $lines.Count;$i++){
  $line=$lines[$i].Trim()
  if($line -match $startRegex){
    $type=$Matches['type'].ToUpperInvariant(); if($type -eq 'PROC'){$type='PROCEDURE'}
    $name=$Matches['name']; if($name -match '^(.*?)(\s+AS\b|\s*\()'){ $name=$Matches[1] }
    $name=($name -replace '--.*$','').Trim(); $name=($name -replace '\s+',' ')
    $tokens=$name.Split(' ',[System.StringSplitOptions]::RemoveEmptyEntries); if($tokens.Count -gt 0){$name=$tokens[0]}
    $key="$type|$name"
    if($key -eq $Target){
      $end=$lines.Count-1
      for($j=$i+1;$j -lt $lines.Count;$j++){
        $t=$lines[$j].Trim()
        if($t -match '^GO$'){ $end=$j; break }
        if($t -match $startRegex){ $end=$j-1; break }
      }
      [string]::Join("`n",$lines[$i..$end]) | Set-Content $Out
      exit 0
    }
  }
}
Write-Error "Target not found: $Target"; exit 1
