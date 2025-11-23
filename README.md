# Proyecto Firmeza

Firmeza es un sistema de gestión empresarial diseñado para negocios del sector de la construcción. Incluye un panel administrativo web y una API RESTful para gestionar productos, clientes, ventas y alquileres.

## Módulos del Proyecto

La solución está organizada en los siguientes proyectos:

-   `Firmeza.Core`: Contiene las entidades del dominio, interfaces y la lógica de negocio principal.
-   `Firmeza.Infrastructure`: Implementa la capa de acceso a datos (repositorios con Entity Framework Core), la conexión a la base de datos PostgreSQL y servicios externos como el envío de correos (SMTP).
-   `Firmeza.web`: Un panel administrativo construido con ASP.NET Core y Razor Pages. Permite la gestión interna del negocio, incluyendo la importación de datos desde Excel.
-   `Firmeza.Api`: Una API RESTful construida con ASP.NET Core Web API. Expone los endpoints para ser consumidos por aplicaciones cliente (como un futuro portal Blazor).
-   `Firmeza.Tests`: Proyecto de pruebas unitarias utilizando xUnit para validar la funcionalidad del sistema.

## Tecnologías Utilizadas

-   **Backend:** .NET 8, ASP.NET Core (Razor Pages & Web API)
-   **Base de Datos:** PostgreSQL
-   **ORM:** Entity Framework Core
-   **Autenticación:** ASP.NET Core Identity con JSON Web Tokens (JWT) para la API.
-   **Mapeo de Objetos:** AutoMapper
-   **Documentación API:** Swashbuckle (Swagger)
-   **Manipulación de Excel:** EPPlus
-   **Generación de PDF:** QuestPDF
-   **Pruebas:** xUnit
-   **Contenerización:** Docker

## Instalación y Ejecución

### Prerrequisitos

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [Docker](https://www.docker.com/products/docker-desktop) (Opcional, para despliegue)

### Base de Datos

La cadena de conexión se encuentra en los archivos `appsettings.json` de los proyectos `Firmeza.web` y `Firmeza.Api`. Actualmente, está configurada para conectarse a una instancia de PostgreSQL en Clever Cloud.

Si deseas usar una base de datos local, asegúrate de tener PostgreSQL instalado y actualiza la cadena de conexión.

### Ejecutar la Aplicación

Puedes ejecutar ambos proyectos simultáneamente desde tu IDE (Rider/Visual Studio) o a través de la línea de comandos:

**Para ejecutar el Panel Web (Razor):**
```bash
dotnet run --project Firmeza.web/Firmeza.web.csproj
```
La aplicación estará disponible en `http://localhost:5168`.

**Para ejecutar la API:**
```bash
dotnet run --project Firmeza.Api/Firmeza.Api.csproj
```
La API estará disponible en `http://localhost:5000`.

## API Endpoints y Versionamiento

La API soporta versionamiento múltiple para garantizar la evolución sin romper la compatibilidad.

### Versión 1 (`/api/v1`)
Endpoints estándar CRUD para operaciones básicas.

### Versión 2 (`/api/v2`) - **¡NUEVO!**
Versión mejorada con funcionalidades avanzadas:
-   **Productos:** Búsqueda avanzada, filtros por precio/stock, estadísticas de inventario, detección de stock bajo.
-   **Clientes:** Historial de compras detallado, identificación de clientes frecuentes, métricas de consumo.

### Documentación Swagger
Puedes explorar y probar ambas versiones interactivamente:
`http://localhost:5000/swagger`
(Usa el selector en la esquina superior derecha para cambiar entre V1 y V2)

## Pruebas Unitarias (Testing)

El proyecto incluye una suite de pruebas unitarias robusta utilizando **xUnit** y **Moq**.

### Cobertura Actual
-   **ProductsV2Controller:** 11 tests (Filtros, Estadísticas, Stock Bajo)
-   **CustomersV2Controller:** 8 tests (Historial, Clientes Frecuentes)

### Ejecutar Pruebas
Para ejecutar las pruebas de la versión 2:
```bash
dotnet test Firmeza.Tests/Firmeza.Tests.csproj --filter "FullyQualifiedName~V2"
```

## Cliente Web (Angular)

El proyecto incluye un cliente web moderno desarrollado en **Angular** que consume la API.
-   Ubicación: `Firmeza.Client`
-   Ejecución: `npm start` (en el directorio del cliente)
-   URL: `http://localhost:4200`

