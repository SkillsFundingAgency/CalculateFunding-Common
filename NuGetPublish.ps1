<#
.SYNOPSIS
Publishes all files matching the filter specified by $packagePathWithFilter. Ignores 409 errors (same version package already present), but throws errors for any other unsuccessful response  
Expects the Nuget.Config specified, to contain a source named 'CalculateFundingService' or as named by parameter sourceNameInNugetConfig
.EXAMPLE
'C:\a\1\s\NuGetPublish.ps1' -feedUrl "https://xxx.pkgs.visualstudio.com/_packaging/xxx/nuget/v3/index.json" -userName VssSessionToken -accessKey ******** -packagePathWithFilter "C:\a\1\s\*.nupkg"
#> 
[CmdletBinding()]
Param(
	[parameter(Mandatory=$true)]
	[string] $feedUrl,
	
	[parameter(Mandatory=$true)]
	[string] $userName,
	
	[parameter(Mandatory=$true)]
	[string] $accessKey,

	[parameter(Mandatory=$true)]
	[string] $packagePathWithFilter,

    [parameter(Mandatory=$false)]
	[string] $sourceNameInNugetConfig = "CalculateFundingService"
)

$currDir = Split-Path $MyInvocation.MyCommand.Path
Write-Host "Current directory is $currDir"
Set-Location -Path $currDir

if (![string]::IsNullOrEmpty($currDir) -And !$currDir.EndsWith("\"))
{
	$currDir = "$currDir\" 
}

Write-Host "Updating source in nuget.config with credentials to nuget feed"
& "$($currDir)nuget.exe" sources Add -NonInteractive -Name $sourceNameInNugetConfig -Source $feedUrl -Username $userName -Password $accessKey

$items = Get-Item -Path $packagePathWithFilter 
ForEach($item in $items)
{
    Write-Host "Pushing to NuGet server, package $($item.Name)"

    $fullFilePath = Path.Combine($item.DirectoryName, $item.Name)

    $ps = new-object System.Diagnostics.Process
    $ps.StartInfo.WorkingDirectory = $currDir 
    $ps.StartInfo.Filename = "$($currDir)nuget.exe"
    $argumentStr = " push ""$($fullFilePath)"" -NonInteractive  -Source ""$feedUrl"" -ApiKey ""VSTS"" -Verbosity Detailed"
    $ps.StartInfo.Arguments = $argumentStr
    $ps.StartInfo.RedirectStandardOutput = $True
    $ps.StartInfo.RedirectStandardError = $True
    $ps.StartInfo.UseShellExecute = $false
    $ps.start()
    $ps.WaitForExit()

    if ($ps.ExitCode -eq 0)
    {
        Write-Host "Package pushed successfully. Exit code is $($ps.ExitCode)" -foregroundcolor yellow
        Write-Host $ps.StandardOutput.ReadToEnd()   
    }
    else
    {
        Write-Host "Package was not pushed. Exit code is $($ps.ExitCode)" -foregroundcolor yellow
        $errMsg = $ps.StandardError.ReadToEnd()
        if ($errMsg -like '*409 (Conflict)*')
        {
            Write-Host "Package with same version already exists. Do you need to update the version number?" -foregroundcolor yellow
            Write-Host $ps.StandardOutput.ReadToEnd()  
            Write-Host $errMsg
        }
        else
        {
            Write-Host $ps.StandardOutput.ReadToEnd() 
            Write-Host $errMsg
            throw $errMsg
        }
    }

    Write-Host "------------------------------------------------------------"    
}