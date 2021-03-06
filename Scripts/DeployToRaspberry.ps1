#Script dependencies:
#Install-Module -Name Posh-SSH

$raspberry = '192.168.0.110'
$remoteProjectPath = '/home/pi/HomeCenter'
$configuration = 'componentConfiguration.json'

$scriptDir = $PSScriptRoot
$projectDir  = Join-Path -Path $scriptDir -ChildPath ..\Utils\HomeCenter.Runner\HomeCenter.Runner.csproj
$publishDir  = Join-Path -Path $scriptDir -ChildPath ..\Utils\HomeCenter.Runner\bin\Debug\netcoreapp2.2\linux-arm\publish
$configurationDir  = Join-Path -Path $scriptDir -ChildPath ..\Configurations
$passwordFile  = Join-Path -Path $scriptDir -ChildPath password.txt

$sw = [Diagnostics.Stopwatch]::StartNew()

Write-Host 'Publish project' -ForegroundColor Green
dotnet publish $projectDir --framework netcoreapp2.2 --runtime linux-arm

Write-Host 'Deploy project' -ForegroundColor Green
$password = Get-Content $passwordFile
$securedPassword = ConvertTo-SecureString $password -AsPlainText -Force
$Credential = New-Object System.Management.Automation.PSCredential ('pi', $securedPassword)

#Deploy application
foreach ($file in (Get-ChildItem $publishDir HomeCenter*.dll)) 
{
    $filePath = Join-Path -Path $publishDir -ChildPath $file
    Write-Host "Copy $filePath => $remoteProjectPath" -ForegroundColor Green 
    
    Set-SCPFile -ComputerName $raspberry -Credential $Credential -LocalFile $filePath -RemotePath $remoteProjectPath
}

#Deploy configuration
$configurationPath = Join-Path -Path $configurationDir -ChildPath $configuration
Write-Host "Copy configuration $configuration" -ForegroundColor Green 
    
Set-SCPFile -ComputerName $raspberry -Credential $Credential -LocalFile $configurationPath -RemotePath $remoteProjectPath


$sw.Stop()
$time = $sw.Elapsed.TotalSeconds
Write-Host "Publish time '$time's" -ForegroundColor Green