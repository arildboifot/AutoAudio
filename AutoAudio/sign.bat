@echo off

set env=%1
set pfx=%2
set pwd=%3
set bin=%~dp0bin\%env%

mage -sign %bin%\AutoAudio.application -CertFile %pfx% -Password %pwd%
mage -sign %bin%\AutoAudio.exe.manifest -CertFile %pfx% -Password %pwd%

signtool sign /v /d AutoAudio /f %pfx% /p %pwd% /tr "http://www.startssl.com/timestamp" %bin%\AutoAudio.exe
signtool sign /v /d AutoAudio /f %pfx% /p %pwd% /tr "http://www.startssl.com/timestamp" %bin%\lib\SoundSwitch.AudioInterface.exe