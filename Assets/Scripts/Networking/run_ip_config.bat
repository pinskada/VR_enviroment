@echo off
SET action=%1
SET scriptdir=%~dp0

IF "%action%"=="static" (
    powershell -Command "Start-Process -FilePath 'powershell.exe' -ArgumentList '-ExecutionPolicy Bypass -File \"\"%scriptdir%setStaticIP.ps1\"\"' -Verb RunAs -Wait"
) ELSE IF "%action%"=="dhcp" (
    powershell -Command "Start-Process -FilePath 'powershell.exe' -ArgumentList '-ExecutionPolicy Bypass -File \"\"%scriptdir%resetDynamicIP.ps1\"\"' -Verb RunAs -Wait"
) ELSE (
    echo Invalid argument. Use 'static' or 'dhcp'.
    pause
)
