-- Script para limpiar productos y vehículos incorrectos
-- y permitir que el SeedingService los regenere correctamente

-- Eliminar detalles de ventas primero (foreign key)
DELETE FROM "SaleDetails";

-- Eliminar ventas
DELETE FROM "Sales";

-- Eliminar rentas
DELETE FROM "Rentals";

-- Eliminar productos
DELETE FROM "Products";

-- Eliminar vehículos
DELETE FROM "Vehicles";

-- Mensaje de confirmación
SELECT 'Base de datos limpiada. Reinicia la aplicación para que el SeedingService genere los datos correctos de ferretería.' AS mensaje;
