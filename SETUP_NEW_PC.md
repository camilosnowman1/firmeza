# Guía de Configuración para Nuevo PC

Esta guía te ayudará a configurar y ejecutar el proyecto **Firmeza** en una computadora nueva.

## 1. Prerrequisitos

Antes de empezar, asegúrate de tener instalado lo siguiente:

1.  **Git**: [Descargar Git](https://git-scm.com/downloads)
2.  **Docker Desktop**: [Descargar Docker Desktop](https://www.docker.com/products/docker-desktop/)
    *   *Importante:* Después de instalar, abre Docker Desktop y asegúrate de que el icono de la ballena esté verde/activo.

## 2. Descargar el Proyecto

Abre una terminal (PowerShell o CMD) y ejecuta:

```bash
# Clonar el repositorio
git clone https://github.com/camilosnowman1/firmeza.git

# Entrar a la carpeta del proyecto
cd firmeza
```

## 3. Ejecutar la Aplicación (Opción Recomendada: Docker)

La forma más fácil de correr todo es usando Docker, ya que no necesitas instalar .NET ni Node.js en tu máquina.

1.  Asegúrate de que **Docker Desktop** esté corriendo.
2.  En la carpeta del proyecto, haz doble clic en el archivo **`RUN_WITH_DOCKER.bat`**.
3.  O si prefieres usar la terminal:
    ```bash
    docker-compose -f docker-compose.yml up --build -d
    ```

### Enlaces de Acceso
Una vez que termine de cargar (puede tardar unos 5-10 minutos la primera vez):

*   **API (Swagger)**: http://localhost:5000/swagger
*   **Panel de Administración**: http://localhost:5001
*   **Cliente (Frontend)**: http://localhost:4200

## 4. Ejecutar Manualmente (Opción Alternativa)

Si prefieres no usar Docker, necesitarás instalar:
*   **.NET 8 SDK**: [Descargar](https://dotnet.microsoft.com/download/dotnet/8.0)
*   **Node.js 20+**: [Descargar](https://nodejs.org/)

Luego corre cada parte en una terminal separada:

**Terminal 1 (API):**
```bash
dotnet run --project Firmeza.Api/Firmeza.Api.csproj
```

**Terminal 2 (Admin):**
```bash
dotnet run --project Firmeza.web/Firmeza.web.csproj
```

**Terminal 3 (Cliente):**
```bash
cd Firmeza.Client
npm install
npm start
```

## Solución de Problemas Comunes

*   **Error: "docker-compose no se reconoce"**: Asegúrate de haber instalado Docker Desktop y reiniciado tu terminal.
*   **Error 500 o "Internal Server Error" al iniciar Docker**: Reinicia Docker Desktop y asegúrate de que esté actualizado.
*   **Puertos ocupados**: Si te dice que el puerto 5000, 5001 o 4200 está en uso, cierra otras aplicaciones que puedan estar usándolos.
