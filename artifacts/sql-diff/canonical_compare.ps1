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

function Canonical([string]$sql){
  if($null -eq $sql){ return '' }
  $x = $sql.ToLowerInvariant()
  $x = [regex]::Replace($x,'--.*?$','', [System.Text.RegularExpressions.RegexOptions]::Multiline)
  $x = [regex]::Replace($x,'/\*.*?\*/','', [System.Text.RegularExpressions.RegexOptions]::Singleline)
  $x = [regex]::Replace($x,'\s+','')
  return $x
}

$targets=@('PROCEDURE|[dbo].[uspEliminarProducto]','PROCEDURE|[dbo].[uspGuardarCredencialesSunat]','PROCEDURE|[dbo].[uspinsertarNotaB]','PROCEDURE|[dbo].[uspListaBajas]')
foreach($t in $targets){
  $a=Get-ObjectBlock 'CLASES_EF/bdactual.sql' $t
  $l=Get-ObjectBlock 'CLASES_EF/bdlocal.sql' $t
  $ca=Canonical $a
  $cl=Canonical $l
  "${t}|equal_after_strip_comments_whitespace=$($ca -eq $cl)|len_actual=$($ca.Length)|len_local=$($cl.Length)"
}
