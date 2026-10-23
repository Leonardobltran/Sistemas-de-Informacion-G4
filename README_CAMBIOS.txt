CAMBIOS APLICADOS

1. Se corrigió el error de Vehículos:
   System.NotImplementedException en ComboTextConverter.ConvertBack.
   Ahora los bindings de la tabla son OneWay y ConvertBack ya no lanza excepción.

2. Se agregó botón Cerrar sesión en la ventana principal.

3. Se agregó modo Administrador / Cliente usando el campo Cargo de la tabla Empleados:
   - Si Cargo contiene "admin" o el usuario es "admin", entra como Administrador.
   - Si no, entra como Cliente.

4. Administrador ve todos los módulos.

5. Cliente ve solo:
   - Ver vehículos
   - Mapa
   En vehículos se abre como solo lectura, sin botones de Nuevo, Guardar ni Eliminar.

6. La conexión quedó configurada para LocalDB:
   Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=huixin_motors;Integrated Security=True;TrustServerCertificate=True

SQL recomendado:
Ejecuta roles_admin_cliente.sql en SQL Server Management Studio para que:
- jsuarez/admin entre como Administrador.
- alopez/12345 entre como Cliente.

CAMBIO NUEVO - FOTOS DESDE ARCHIVOS:
- En Vehiculos se agrego el boton "Buscar foto".
- Permite seleccionar una imagen desde el gestor de archivos de Windows.
- La imagen se copia a una carpeta estable del usuario: AppData\Local\HuixinMotors\FotosVehiculos.
- Luego se debe presionar "Guardar" para guardar la ruta en la base de datos.
