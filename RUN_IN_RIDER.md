# Cómo ejecutar Firmeza en JetBrains Rider

Este proyecto consta de tres partes principales:
1.  **Firmeza.Api**: El Backend (API REST).
2.  **Firmeza.web**: La aplicación administrativa (ASP.NET Core MVC/Razor).
3.  **Firmeza.Client**: El Frontend para clientes (Angular).

Sigue estos pasos para ejecutar todo el sistema en tu entorno local.

## 1. Configuración del Backend y Admin (C#)

1.  **Abrir la Solución**: Abre el archivo `Firmeza.sln` con JetBrains Rider.
2.  **Restaurar Paquetes**: Rider debería restaurar automáticamente los paquetes NuGet. Si no, haz clic derecho en la solución y selecciona "Restore NuGet Packages".
3.  **Configurar Ejecución Múltiple**:
    *   Ve al menú desplegable de configuraciones de ejecución (arriba a la derecha, cerca del botón "Play").
    *   Selecciona **"Edit Configurations..."**.
    *   Haz clic en el botón **"+"** y selecciona **"Compound"**.
    *   Nombra la configuración (ej. "Todo el Backend").
    *   En la lista de proyectos, marca las casillas para:
        *   `Firmeza.Api`
        *   `Firmeza.web`
    *   Haz clic en **Apply** y **OK**.
4.  **Ejecutar**: Selecciona tu nueva configuración "Todo el Backend" y presiona el botón **Run** (Play).
    *   Esto abrirá dos ventanas de navegador (o pestañas) con la API (Swagger) y el sitio Administrativo.

> **Nota sobre la Base de Datos**: Actualmente, `appsettings.json` en ambos proyectos apunta a una base de datos remota en Clever Cloud. Si deseas usar una base de datos local, asegúrate de tener PostgreSQL instalado y actualiza la cadena de conexión `DefaultConnection` en `Firmeza.Api/appsettings.json` y `Firmeza.web/appsettings.json`.

## 2. Configuración del Frontend (Angular)

El cliente de Angular se ejecuta por separado usando Node.js.

1.  **Abrir Terminal**: En Rider, abre la pestaña "Terminal" (abajo a la izquierda).
2.  **Navegar al directorio**:
    ```bash
    cd Firmeza.Client
    ```
3.  **Instalar Dependencias** (solo la primera vez):
    ```bash
    npm install
    ```
4.  **Ejecutar el Servidor de Desarrollo**:
    ```bash
    npm start
    ```
    *   O alternativamente: `ng serve -o`
5.  **Acceder**: Una vez compile, abre tu navegador en `http://localhost:4200`.

## Resumen de URLs Típicas

*   **API (Swagger)**: Generalmente en `http://localhost:5000/swagger` o `https://localhost:5001/swagger` (revisa la consola de Rider para ver el puerto exacto).
*   **Admin Web**: Generalmente en `http://localhost:5000` o un puerto similar.
*   **Cliente Angular**: `http://localhost:4200`.
