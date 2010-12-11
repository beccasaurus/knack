@echo off

SET NUNIT_CONSOLE = tools\nunit-console.exe

RMDIR /Q bin

MSBuild && tools\nunit-color-console.exe bin\Debug\Owin.Common.Specs.dll