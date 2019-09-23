#NOTE: Make sure to run as Administrator

param
(
    [Parameter(Mandatory=$True)] [ValidateNotNull()] [string]$exePath,
    [Parameter(Mandatory=$True)] [ValidateNotNull()] [string]$domainAndUser
)

$acl = Get-Acl $exePath
$aclRuleArgs = $domainAndUser, "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
$acl.SetAccessRule($accessRule)
$acl | Set-Acl $exePath

New-Service -Name HostAsWindowsService -BinaryPathName $exePath\MyProject.exe -Credential $domainAndUser -Description "Hosting App .NET Core as a windows service" -DisplayName "Host App Test on Windows" -StartupType Automatic
