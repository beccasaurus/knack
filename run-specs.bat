@echo off

SET NUNIT_CONSOLE = tools\nunit-console.exe

RMDIR /Q bin

MSBuild && tools\nunit-color-console.exe /labels bin\Debug\Owin.Common.Specs.dll