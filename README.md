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

## API Endpoints

La API está versionada (`/api/v1`) y documentada con Swagger. Una vez que la API esté en ejecución, puedes acceder a la documentación interactiva en:

`http://localhost:5000/swagger`

### Endpoints Principales

-   `api/v1/Auth/register`: Registro de nuevos usuarios (con rol "Cliente").
-   `api/v1/Auth/login`: Autenticación y obtención de token JWT.
-   `api/v1/Products`: CRUD completo para la gestión de productos.
-   `api/v1/Customers`: CRUD completo para la gestión de clientes.
-   `api/v1/Sales`: CRUD completo para la gestión de ventas.

Para acceder a los endpoints protegidos, debes incluir el token JWT en la cabecera `Authorization` con el esquema `Bearer`.
