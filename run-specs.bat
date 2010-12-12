@echo off

SET NUNIT_CONSOLE = tools\nunit-console.exe

RMDIR /Q bin

MSBuild && tools\nunit-color-console.exe /labels bin\Debug\Owin.Common.Specs.dll bin\Debug\Owin.Handlers.Cgi.Specs.dll bin\Debug\Owin.Handlers.AspNet.Specs.dll bin\Debug\Owin.Test.Specs.dll
