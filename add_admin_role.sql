-- Script para agregar rol de Admin al usuario camilosnowman@gmail.com

-- Primero, necesitamos el ID del usuario
-- SELECT "Id" FROM "AspNetUsers" WHERE "Email" = 'camilosnowman@gmail.com';

-- Luego, necesitamos el ID del rol Admin
-- SELECT "Id" FROM "AspNetRoles" WHERE "Name" = 'Admin';

-- Finalmente, agregamos la relación entre usuario y rol
-- Reemplaza <USER_ID> y <ROLE_ID> con los IDs obtenidos arriba

-- INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
-- VALUES ('<USER_ID>', '<ROLE_ID>')
-- ON CONFLICT DO NOTHING;

-- Script completo con subconsultas (ejecuta este):
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT u."Id", r."Id"
FROM "AspNetUsers" u
CROSS JOIN "AspNetRoles" r
WHERE u."Email" = 'camilosnowman@gmail.com' 
  AND r."Name" = 'Admin'
  AND NOT EXISTS (
    SELECT 1 FROM "AspNetUserRoles" ur 
    WHERE ur."UserId" = u."Id" AND ur."RoleId" = r."Id"
  );
