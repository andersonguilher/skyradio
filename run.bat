@echo off
title SkyRadio - Development Runner

echo ========================================
echo  Iniciando Ambiente de Teste SkyRadio
echo ========================================
echo.

REM --- 1. Iniciar o Servidor Node.js ---
REM Verifica se o servidor ja esta rodando para nao abrir varias janelas
tasklist /FI "WINDOWTITLE eq SkyRadio Server" 2>NUL | find /I /N "node.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [INFO] Servidor Node.js ja esta em execucao.
) else (
    echo [1/3] Iniciando servidor Node.js (em uma nova janela)...
    start "SkyRadio Server" cmd /c "node server/server.js & pause"
    echo [INFO] Aguardando 2 segundos para o servidor inicializar...
    timeout /t 2 /nobreak > nul
)

echo.
echo [2/3] Compilando o cliente SkyRadio (Debug)...
dotnet build -c Debug

REM Verifica se a compilacao falhou
if %errorlevel% neq 0 (
    echo.
    echo **********************************
    echo *   ERRO NA COMPILACAO.          *
    echo *   Verifique as mensagens acima.*
    echo **********************************
    pause
    exit /b
)

echo.
echo [3/3] Iniciando cliente SkyRadio...
start "SkyRadio Client" "bin\x64\Debug\net472\SkyRadio.exe"

echo.
echo Ambiente iniciado. Para testar com dois clientes, execute este arquivo novamente.
echo.
pause