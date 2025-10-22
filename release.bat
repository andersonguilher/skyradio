@echo off
title SkyRadio - Release Build

echo ========================================
echo  Compilando versao de Lancamento (Release)
echo ========================================

dotnet build -c Release

echo.
echo Compilacao finalizada. Os arquivos estao em: \bin\x64\Release\net472\
pause