@echo off

set env=%1
set pfx=%2
set pwd=%3
set v=%4

set path=%path%;C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools;C:\Program Files (x86)\Windows Kits\8.0\bin\x86

set bin=%~dp0AutoAudio\bin\%env%
set rootDeploy=%~dp0deploy
set deploy=%rootDeploy%\%v%

md %deploy%

xcopy %bin%\AutoAudio.exe %deploy% /S /Y
xcopy %bin%\NLog.dll %deploy% /S /Y
xcopy %bin%\NLog.config %deploy% /S /Y
xcopy %bin%\AutoAudio.exe.config %deploy% /S /Y

xcopy %bin%\lib\SoundSwitch.AudioInterface.exe %deploy%\lib\ /S /Y

xcopy tools\bootstrapper\setup.exe %rootDeploy% /Y

signtool sign /v /d AutoAudio /f %pfx% /p %pwd% /tr "http://www.startssl.com/timestamp" "%rootDeploy%\setup.exe"
for /R "%deploy%" %%i in (*exe*) do signtool sign /v /d AutoAudio /f %pfx% /p %pwd% /tr "http://www.startssl.com/timestamp" "%%i"
for /R "%deploy%" %%i in (*dll*) do signtool sign /v /d AutoAudio /f %pfx% /p %pwd% /tr "http://www.startssl.com/timestamp" "%%i"

mage -New Application -Processor x86 -ToFile %deploy%\AutoAudio.exe.manifest -Name "AutoAudio" -Version %v% -FromDirectory %deploy%
mage -sign %deploy%\AutoAudio.exe.manifest -CertFile %pfx% -Password %pwd%

mage -New Deployment -Processor x86 -Install true -Publisher "CottonCactus" -ProviderUrl "http://www.boifotsoftware.no/apps/AutoAudio/AutoAudio.application" -AppManifest %deploy%\AutoAudio.exe.manifest -ToFile %rootDeploy%\AutoAudio.application
mage -sign %rootDeploy%\AutoAudio.application -CertFile %pfx% -Password %pwd%
