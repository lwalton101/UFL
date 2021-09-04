@ECHO OFF
rmdir /s Builds\netcoreapp3.1
dotnet publish -r win10-x64 --self-contained true -c release
PAUSE