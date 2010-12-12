@echo off
xcopy ..\..\bin\Debug\*.dll .
csc RunKayakApp.cs /r:owin.dll  /r:Owin.Common.dll  /r:Owin.Handlers.Kayak.dll
RunKayakApp.exe
