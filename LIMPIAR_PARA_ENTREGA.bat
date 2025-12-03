@echo off
echo ========================================================
echo PREPARANDO PROYECTO PARA ENTREGA - LIMPIEZA
echo ========================================================
echo.
echo 1. Borrando carpetas bin y obj (compilados)...
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

echo.
echo 2. Borrando node_modules (dependencias de Angular)...
echo    (Esto puede tardar un poco, ten paciencia)
if exist "Firmeza.Client\node_modules" (
    rmdir /s /q "Firmeza.Client\node_modules"
)

echo.
echo 3. Borrando configuraciones de IDE (.idea, .vs)...
if exist ".idea" rmdir /s /q ".idea"
if exist ".vs" rmdir /s /q ".vs"

echo.
echo ========================================================
echo ¡LISTO! PROYECTO LIMPIO.
echo Ahora la carpeta pesara mucho menos.
echo Ya puedes comprimir la carpeta 'firmeza' en .zip
echo ========================================================
pause
