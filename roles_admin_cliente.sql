USE huixin_motors;
GO

-- Usuario administrador para ver todas las opciones del sistema
UPDATE Empleados
SET Cargo = 'Administrador', Estado = 1
WHERE Usuario = 'jsuarez';
GO

-- Usuario cliente para ver solo opciones limitadas
UPDATE Empleados
SET Cargo = 'Cliente', Estado = 1
WHERE Usuario = 'alopez';
GO

-- Si quieres crear un administrador nuevo, descomenta este bloque:
-- IF NOT EXISTS (SELECT 1 FROM Empleados WHERE Usuario = 'admin')
-- BEGIN
--     INSERT INTO Empleados (Nombres, Apellidos, Cargo, Telefono, Email, Usuario, Clave, Estado)
--     VALUES ('Administrador', 'Sistema', 'Administrador', NULL, NULL, 'admin', '123456', 1);
-- END
-- GO

SELECT IdEmpleado, Nombres, Apellidos, Cargo, Usuario, Clave, Estado
FROM Empleados;
GO
