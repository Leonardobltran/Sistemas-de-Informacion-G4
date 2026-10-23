# Huixin Motors — versión mejorada del código fuente

## Cambios aplicados
- Nueva paleta corporativa turquesa y estilos del menú centralizados en `HuixinMotors.PL/Helpers/ThemeHelper.cs`.
- Logotipo existente agregado a los encabezados compartidos de los módulos CRUD.
- Iconos de navegación por módulo en el menú principal.
- Registro de ventas: venta e inventario se modifican dentro de una única transacción SQL y se bloquea la doble venta concurrente.
- Eliminación de venta: devolución del vehículo en la misma transacción (se conserva la eliminación física original; para producción se recomienda anulación y auditoría).
- Contraseñas: PBKDF2-SHA256 con salt e iteraciones; migración gradual de claves antiguas tras inicio de sesión exitoso. La primera vez las cuentas anteriores todavía existen en texto plano hasta que inicien sesión: planificar migración forzada y cambio de claves.
- Validaciones extra para precio y referencias en ventas.

## Requisitos para probar
1. Windows con SDK .NET 8, SQL Server LocalDB y acceso a NuGet.
2. Abrir `HuixinMotors.sln` en Visual Studio; restaurar paquetes y compilar.
3. Crear la base de datos con `huixin_motors_database.sql` y, si corresponde, `roles_admin_cliente.sql` después de revisar los datos de prueba.
4. Revisar la cadena de conexión en `HuixinMotors.PL/appsettings.json`.
5. Ejecutar y comprobar inicio de sesión, CRUD, ventas, inventario, reportes y cierre de sesión con una base de datos de prueba.

## Pendientes IMPORTANTES de producción
- No se pudo compilar ni ejecutar aquí: el entorno no trae `dotnet` ni SQL Server. No se garantiza ejecución al 100%.
- Sustituir el rol deducido del usuario/cargo por permisos persistidos y validados en la capa de negocio.
- Reemplazar eliminación física de ventas por anulaciones auditables y diseñar comprobantes.
- Evitar mostrar valores hash de contraseñas en el CRUD de empleados; separar DTOs de administración y autenticación.
- Mejorar las cuadrículas de formularios que usan `Canvas` de posiciones fijas; no son responsivas.
- Agregar pantallas de configuración de temas, reportes PDF y Excel, controles de búsqueda y pruebas automatizadas.
- Asegurar backups y gestión de secretos/credenciales; revisar cumplimiento tributario antes de emitir facturas.

### Archivos importantes
- Colores y logotipo: `HuixinMotors.PL/Helpers/ThemeHelper.cs`
- Menú e iconos: `HuixinMotors.PL/Views/MainWindow.cs`
- CRUD: `HuixinMotors.PL/Views/GenericCrudWindow.cs`
- Login: `HuixinMotors.PL/Views/LoginWindow.cs`
- Ventas transaccionales: `HuixinMotors.DAL/Repositories/VentaRepository.cs`
- Hash de claves: `HuixinMotors.DAL/Security/PasswordHasher.cs`
