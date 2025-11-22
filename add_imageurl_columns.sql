-- Migration: Add ImageUrl to Products and Vehicles
-- Date: 2025-11-22

-- Add ImageUrl column to Products table
ALTER TABLE "Products" 
ADD COLUMN "ImageUrl" VARCHAR(500) NULL;

-- Add ImageUrl column to Vehicles table  
ALTER TABLE "Vehicles"
ADD COLUMN "ImageUrl" VARCHAR(500) NULL;
