@echo off
REM
REM The System.Windows assembly that ships with Silverlight needs to be 
REM referenced by the design-time experience DLLs. This batch file locates the 
REM latest Silverlight assembly and copies it into the local .\Binaries\ 
REM directory to simplify the build process.

SET THIS_DIR=%~dp0
IF EXIST %THIS_DIR%Binaries\System.Windows.dll GOTO Finished

SET LocalProgramFiles=%ProgramFiles(x86)%
IF "%LocalProgramFiles%" == "" SET LocalProgramFiles=%ProgramFiles%

SET SilverlightDirectory="%LocalProgramFiles%\Microsoft Silverlight\"
for /f %%a IN ('dir /b/d %SilverlightDirectory%\2.*') do SET SystemWindowsLocation=%%a
copy %SilverlightDirectory%%SystemWindowsLocation%\System.Windows.dll "%THIS_DIR%Binaries\"

:Finished