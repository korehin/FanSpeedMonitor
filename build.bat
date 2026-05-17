@echo off

echo =========================
echo MSI_Claw_Fan_PRM BUILD
echo =========================
echo.

REM Очистка старых сборок
if exist "bin\Release" rd /s /q "bin\Release"
if exist "bin\x64\Release" rd /s /q "bin\x64\Release"

REM Сборка минимального EXE (требует .NET Runtime на целевой машине)
dotnet publish -c Release -r win-x64 ^
  --self-contained false ^
  -p:PublishSingleFile=true ^
  -p:DebugType=None ^
  -p:DebugSymbols=false

if errorlevel 1 (
    echo.
    echo BUILD FAILED!
    pause
    exit /b 1
)

REM Копирование EXE в корень проекта
if exist "bin\Release\net8.0-windows\win-x64\publish\MSI_Claw_Fan_PRM.exe" (
    copy /y "bin\Release\net8.0-windows\win-x64\publish\MSI_Claw_Fan_PRM.exe" "MSI_Claw_Fan_PRM.exe"
) else if exist "bin\x64\Release\net8.0-windows\win-x64\publish\MSI_Claw_Fan_PRM.exe" (
    copy /y "bin\x64\Release\net8.0-windows\win-x64\publish\MSI_Claw_Fan_PRM.exe" "MSI_Claw_Fan_PRM.exe"
) else (
    echo ERROR: EXE not found!
)

echo.
echo =========================
echo DONE
echo =========================
echo.

echo EXE copied to:
echo %cd%\MSI_Claw_Fan_PRM.exe
echo.
echo Size optimized: requires .NET 8 Runtime on target machine
echo.

pause