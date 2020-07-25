if (Test-Path .\bin\MovieList\) 
{
    Remove-Item -Path .\bin\MovieList\ -Recurse
}

dotnet publish . --configuration Release --runtime win10-x64 --self-contained true --output .\bin\MovieList --nologo -p:Platform=x64 -p:PublishTrimmed=true

Remove-Item -Path .\bin\MovieList\ -Include *.pdb, *.xml

if (Test-Path .\bin\MovieList.zip) 
{
    Remove-Item -Path .\bin\MovieList.zip
}

Compress-Archive -Path .\bin\MovieList\ -DestinationPath .\bin\MovieList.zip

Remove-Item -Path .\bin\MovieList\ -Recurse
