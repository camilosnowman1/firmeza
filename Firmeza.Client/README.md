# Firmeza Client Module

Este proyecto es el módulo frontend para la plataforma Firmeza, desarrollado en Angular. Permite a los clientes registrarse, iniciar sesión, ver productos, agregar al carrito y realizar compras.

## Características

- **Autenticación**: Registro e inicio de sesión con JWT.
- **Catálogo de Productos**: Visualización de productos disponibles.
- **Carrito de Compras**: Gestión de items, cantidades y cálculo de totales.
- **Compras**: Creación de pedidos y envío de comprobantes por correo.
- **Diseño**: Interfaz responsiva utilizando Bootstrap.

## Requisitos Previos

- Node.js (v18 o superior)
- Angular CLI (`npm install -g @angular/cli`)
- Backend de Firmeza (`Firmeza.Api`) ejecutándose en `http://localhost:5000`.

## Instalación

1. Clonar el repositorio.
2. Navegar a la carpeta del proyecto:
   ```bash
   cd Firmeza.Client
   ```
3. Instalar dependencias:
   ```bash
   npm install
   ```

## Ejecución

### Desarrollo Local

1. Asegúrate de que la API esté corriendo.
2. Inicia el servidor de desarrollo:
   ```bash
   ng serve
   ```
3. Abre el navegador en `http://localhost:4200`.

### Docker

1. Construir la imagen:
   ```bash
   docker build -t firmeza-client .
   ```
2. Ejecutar el contenedor:
   ```bash
   docker run -d -p 80:80 firmeza-client
   ```

## Pruebas

Ejecutar pruebas unitarias:
```bash
ng test
```
