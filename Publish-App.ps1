if (Test-Path .\bin\Cineaste\) 
{
    Get-Childitem .\bin\Cineaste\ -Recurse | ForEach-Object { 
        Remove-Item $_.FullName -Force -Recurse
    }

    Remove-Item -Recurse -Force -Path .\bin\Cineaste\
}

dotnet publish .\Cineaste\Cineaste.csproj --configuration Release --runtime win10-x64 --self-contained true `
--output .\bin\Cineaste --nologo -p:Platform=x64 -p:PublishSingleFile=true

Remove-Item -Path .\bin\Cineaste\Cineaste.pdb
Remove-Item -Path .\bin\Cineaste\Cineaste.Core.pdb
Remove-Item -Path .\bin\Cineaste\Cineaste.Data.pdb

if (Test-Path .\bin\Cineaste.zip)
{
    Remove-Item -Path .\bin\Cineaste.zip
}

Compress-Archive -Path .\bin\Cineaste\ -DestinationPath .\bin\Cineaste.zip

Get-Childitem .\bin\Cineaste\ -Recurse | ForEach-Object { 
    Remove-Item $_.FullName -Force -Recurse
}

Remove-Item -Recurse -Force -Path .\bin\Cineaste\
