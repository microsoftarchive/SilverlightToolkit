@echo off
REM
REM The System.Windows assembly that ships with Silverlight needs to be 
REM referenced by the design-time experience DLLs. This batch file locates the 
REM latest Silverlight assembly and copies it into the local .\Binaries\ 
REM directory to simplify the build process.
REM
REM This file also locates Expression Blend 3 Preview reference assemblies
REM in the same manner.


SET LocalProgramFiles=%ProgramFiles(x86)%
IF "%LocalProgramFiles%" == "" SET LocalProgramFiles=%ProgramFiles%

SET THIS_DIR=%~dp0
IF EXIST %THIS_DIR%Binaries\System.Windows.dll GOTO TryNextAssembly

SET SilverlightDirectory="%LocalProgramFiles%\Microsoft Silverlight\"
for /f %%a IN ('dir /b/d %SilverlightDirectory%\3.*') do SET SystemWindowsLocation=%%a
copy %SilverlightDirectory%%SystemWindowsLocation%\System.Windows.dll "%THIS_DIR%Binaries\"

:TryNextAssembly

SET Blend3Directory="%LocalProgramFiles%\Microsoft Expression\Blend 3 Preview\"

IF EXIST %THIS_DIR%Binaries\Blend3\Microsoft.Windows.Design.Extensibility.dll GOTO TryNextAssembly2
IF NOT EXIST %THIS_DIR%Binaries\Blend3 mkdir %THIS_DIR%Binaries\Blend3\
IF EXIST %Blend3Directory%Microsoft.Windows.Design.Extensibility.dll copy %Blend3Directory%Microsoft.Windows.Design.Extensibility.dll %THIS_DIR%Binaries\Blend3\

:TryNextAssembly2
IF EXIST %THIS_DIR%Binaries\Blend3\Microsoft.Windows.Design.Interaction.dll GOTO Finished
IF NOT EXIST %THIS_DIR%Binaries\Blend3 mkdir %THIS_DIR%Binaries\Blend3\
IF EXIST %Blend3Directory%Microsoft.Windows.Design.Interaction.dll copy %Blend3Directory%Microsoft.Windows.Design.Interaction.dll %THIS_DIR%Binaries\Blend3\

:Finished
