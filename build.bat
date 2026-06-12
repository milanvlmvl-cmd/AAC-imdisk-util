@echo off
title AAC Raid Smoother Compiler
set RESOURCES=
echo ===================================================
echo   AAC Raid Smoother - Quick Compilation Script
echo ===================================================
echo.

:: Zoek naar de .NET Framework C# compiler (csc.exe)
set CSC_PATH="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist %CSC_PATH% (
    set CSC_PATH="C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"
)

if not exist %CSC_PATH% (
    echo [FOUT] C# Compiler csc.exe kon niet worden gevonden.
    echo Zorg ervoor dat .NET Framework 4.8 of hoger is geinstalleerd.
    pause
    exit /b 1
)

echo [+] C# Compiler gevonden op: %CSC_PATH%
echo [+] Controleren op ingebedde driverbestanden...

:: Bereid de resource-parameters voor als de driverbestanden aanwezig zijn
set RESOURCES=
if exist imdisk.exe (
    echo [INFO] imdisk.exe gevonden. Wordt ingebed in executable.
    set RESOURCES=%RESOURCES% /res:imdisk.exe,imdisk.exe
) else (
    echo [WAARSCHUWING] imdisk.exe mist. De driver kan niet automatisch worden geinstalleerd als deze niet vooraf op het doelsysteem staat.
)
if exist imdisk.sys (
    echo [INFO] imdisk.sys gevonden. Wordt ingebed in executable.
    set RESOURCES=%RESOURCES% /res:imdisk.sys,imdisk.sys
)
if exist imdisk.inf (
    echo [INFO] imdisk.inf gevonden. Wordt ingebed in executable.
    set RESOURCES=%RESOURCES% /res:imdisk.inf,imdisk.inf
)

echo [+] Compileren van de broncode...
%CSC_PATH% /target:winexe /out:AAC_Raid_Smoother.exe /win32manifest:app.manifest /optimize+ %RESOURCES% Form1.cs Program.cs

if %errorlevel% equ 0 (
    echo.
    echo ===================================================
    echo [+] SUCCES: AAC_Raid_Smoother.exe is succesvol gebouwd.
    echo ===================================================
) else (
    echo.
    echo [FOUT] Compilatie mislukt met foutcode: %errorlevel%
)
echo.
pause
