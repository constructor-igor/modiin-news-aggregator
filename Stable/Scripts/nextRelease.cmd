@echo off
SET SOURCE_ROOT=..\Sources\ModiinNewsAggregator
SET RELEASE_OUTPUT_FOLDER=%SOURCE_ROOT%\ModiinNewsAggregator\bin\Release
SET REBUILD_OUTPUTFILE=ReBuildAllEngine.log
SET INSTALL_BINARY_FOLDER=..\Release
SET DROPBOX_TARGET=D:\My\MyDropbox\Dropbox\My\MyProjects\NewsAggregator\DeployPackage

@echo Delete folders (Release and Install)
rmdir /Q /S "%RELEASE_OUTPUT_FOLDER%" >%REBUILD_OUTPUTFILE%
rmdir /Q /S "%INSTALL_BINARY_FOLDER%" >>%REBUILD_OUTPUTFILE%
IF %ERRORLEVEL% NEQ 0 GOTO EndError

echo Building Solution
"%VS100COMNTOOLS%\..\IDE\DevEnv.exe" "%SOURCE_ROOT%\ModiinNewsAggregator.sln" /out %REBUILD_OUTPUTFILE% /Rebuild Release
IF %ERRORLEVEL% NEQ 0 GOTO EndError

@echo Delete "development" twitter.config file
del /Q "%RELEASE_OUTPUT_FOLDER%\twitter.config" >>%REBUILD_OUTPUTFILE%
IF %ERRORLEVEL% NEQ 0 GOTO EndError

@echo Create Install folder
mkdir "%INSTALL_BINARY_FOLDER%"
IF %ERRORLEVEL% NEQ 0 GOTO EndError

@echo Copy files to Install folder
xcopy /e "%RELEASE_OUTPUT_FOLDER%" "%INSTALL_BINARY_FOLDER%" >> %REBUILD_OUTPUTFILE%
IF %ERRORLEVEL% NEQ 0 GOTO EndError

@echo Copy files to Dropbox Install folder
xcopy /e /Y "%INSTALL_BINARY_FOLDER%" "%DROPBOX_TARGET%" >> %REBUILD_OUTPUTFILE%
IF %ERRORLEVEL% NEQ 0 GOTO EndError


:EndOK
echo Finished
GOTO End

:EndError
echo Finished With Error - Check Errors

:End
pause