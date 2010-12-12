@echo off
mkdir Bin
xcopy ..\..\bin\Debug\*.dll .
C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe /target:library AspNetApp.cs /r:owin.dll  /r:Owin.Common.dll  /r:Owin.Handlers.AspNet.dll
xcopy *.dll Bin\
pause