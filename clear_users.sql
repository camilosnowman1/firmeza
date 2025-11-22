-- Delete all users and user roles
DELETE FROM "AspNetUserRoles";
DELETE FROM "AspNetUsers";

-- Reset identity sequences if needed
-- ALTER SEQUENCE "AspNetUsers_Id_seq" RESTART WITH 1;
