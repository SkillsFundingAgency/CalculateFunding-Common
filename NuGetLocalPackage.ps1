$currDir = Split-Path $MyInvocation.MyCommand.Path

# To make the script re-useable let’s fetch the project file name automatically
$files = get-childitem -recurse -Path "$($currDir)\*\*.csproj"

#dotnet nuget locals all --clear

foreach ($file in $files)
{
    pushd $($file.directory)
    # Package the project. Note that $projectFile is a FileInfo object so we need to get the name property for the command
    dotnet publish "$($file.directory)\$($file.name)" --configuration Debug
    dotnet pack --output "C:\Projects\Common" /p:Configuration=Debug
}

