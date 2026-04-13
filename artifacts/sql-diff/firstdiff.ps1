function Get-ObjectBlock([string]$path,[string]$target){
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
        $end=$lines.Count-1
        for($j=$i+1;$j -lt $lines.Count;$j++){
          $t=$lines[$j].Trim()
          if($t -match '^GO$'){ $end=$j; break }
          if($t -match $startRegex){ $end=$j-1; break }
        }
        return [string]::Join("`n",$lines[$i..$end])
      }
    }
  }
  return $null
}

function Canonical([string]$sql){
  $x=$sql.ToLowerInvariant()
  $x=[regex]::Replace($x,'--.*?$','',[System.Text.RegularExpressions.RegexOptions]::Multiline)
  $x=[regex]::Replace($x,'/\*.*?\*/','',[System.Text.RegularExpressions.RegexOptions]::Singleline)
  $x=[regex]::Replace($x,'\s+','')
  return $x
}

$targets=@('PROCEDURE|[dbo].[uspEliminarProducto]','PROCEDURE|[dbo].[uspListaBajas]','PROCEDURE|[dbo].[uspinsertarNotaB]')
foreach($t in $targets){
  $a=Canonical (Get-ObjectBlock 'CLASES_EF/bdactual.sql' $t)
  $l=Canonical (Get-ObjectBlock 'CLASES_EF/bdlocal.sql' $t)
  if($a -eq $l){ "${t}|equal"; continue }
  $min=[Math]::Min($a.Length,$l.Length)
  $idx=-1
  for($i=0;$i -lt $min;$i++){ if($a[$i] -ne $l[$i]){ $idx=$i; break } }
  if($idx -eq -1){ $idx=$min }
  $start=[Math]::Max(0,$idx-80); $len=[Math]::Min(220,[Math]::Min($a.Length,$l.Length)-$start)
  "${t}|first_diff_index=$idx|lenA=$($a.Length)|lenL=$($l.Length)"
  "A_SNIP=" + $a.Substring($start,$len)
  "L_SNIP=" + $l.Substring($start,$len)
}
