@echo off

echo f | xcopy /s /y /i /f ..\FKCashierCommon\Bin\x86\Debug\*.dll Bin\x86\Debug
echo f | xcopy /s /y /i /f ..\FKCashierCommon\Bin\x86\Debug\*.xml Bin\x86\Debug
echo f | xcopy /s /y /i /f ..\FKCashierCommon\Bin\x86\Debug\*.sys Bin\x86\Debug
echo f | xcopy /s /y /i /f ..\FKCashierCommon\Depend\IEDriverServer.exe Bin\x86\Debug

echo f | xcopy /s /y /i /f ..\FKCashierCommon\Bin\x86\Release\*.dll Bin\x86\Release
echo f | xcopy /s /y /i /f ..\FKCashierCommon\Bin\x86\Release\*.xml Bin\x86\Release
echo f | xcopy /s /y /i /f ..\FKCashierCommon\Bin\x86\Release\*.sys Bin\x86\Release
echo f | xcopy /s /y /i /f ..\FKCashierCommon\Depend\IEDriverServer.exe Bin\x86\Release

echo f | xcopy /s /y /i /f ..\FKCashierCommon\Bin\x86\Debug\*.dll Depend
echo f | xcopy /s /y /i /f ..\FKCashierCommon\Bin\x86\Debug\*.xml Depend
echo f | xcopy /s /y /i /f ..\FKCashierCommon\Bin\x86\Debug\*.sys Depend
echo f | xcopy /s /y /i /f ..\FKCashierCommon\Depend\IEDriverServer.exe Depend


pause