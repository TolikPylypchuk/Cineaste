if (Test-Path .\bin\MovieList\) 
{
    Get-Childitem .\bin\MovieList\ -Recurse | ForEach-Object { 
        Remove-Item $_.FullName -Force -Recurse
    }

    Remove-Item -Recurse -Force -Path .\bin\MovieList\
}

dotnet publish .\MovieList\MovieList.csproj --configuration Release --runtime win10-x64 --self-contained `
--output .\bin\MovieList --nologo -p:Platform=x64 -p:PublishSingleFile=true

Remove-Item -Path .\bin\MovieList\ -Include *.pdb, *.xml

if (Test-Path .\bin\MovieList.zip) 
{
    Remove-Item -Path .\bin\MovieList.zip
}

Compress-Archive -Path .\bin\MovieList\ -DestinationPath .\bin\MovieList.zip

Get-Childitem .\bin\MovieList\ -Recurse | ForEach-Object { 
    Remove-Item $_.FullName -Force -Recurse
}

Remove-Item -Recurse -Force -Path .\bin\MovieList\
