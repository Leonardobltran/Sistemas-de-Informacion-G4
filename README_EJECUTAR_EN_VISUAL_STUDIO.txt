HUIXIN MOTORS - COMO EJECUTAR EN VISUAL STUDIO

1) Requisitos
- Visual Studio 2022.
- Workload: .NET desktop development / Desarrollo de escritorio con .NET.
- .NET 8 SDK instalado.
- SQL Server Express, Developer o LocalDB.
- Internet para restaurar paquetes NuGet y para que cargue el mapa de OpenStreetMap.

2) Abrir el proyecto
- Abre el archivo HuixinMotors.sln.
- Si Visual Studio pregunta por restaurar paquetes NuGet, acepta.
- En el Explorador de soluciones, clic derecho en HuixinMotors.PL > Set as Startup Project / Establecer como proyecto de inicio.

3) Crear la base de datos
- Abre SQL Server Management Studio o Azure Data Studio.
- Ejecuta el archivo huixin_motors_database.sql.
- Ese script crea la base huixin_motors, tablas, vistas, procedimiento almacenado y un usuario inicial.

4) Configurar conexión
- Abre HuixinMotors.PL/appsettings.json.
- Cambia el servidor, usuario y contraseña según tu SQL Server.
- Si usas SQL Server local con usuario sa, verifica que SQL Server acepte autenticación SQL.
- Si usas Windows Authentication, usa una cadena similar:
  Data Source=localhost;Initial Catalog=huixin_motors;Integrated Security=True;TrustServerCertificate=True

5) Ejecutar
- Presiona F5 o el botón verde de ejecutar.
- Usuario inicial: admin
- Clave inicial: 123456

6) Si no carga el mapa
- El mapa usa tiles de OpenStreetMap por internet.
- Si no hay internet o el firewall bloquea la conexión, se verá gris.
- Para que aparezcan marcadores, las sucursales deben tener Latitud y Longitud.
- El script ya crea sucursales con coordenadas de ejemplo.

Correcciones incluidas en esta versión
- Se agregó HuixinMotors.sln para abrir directo en Visual Studio.
- Se corrigió un error de compilación en MapaControl.cs: había un else duplicado.
- Se corrigieron referencias a BusinessRuleException en ventanas de la capa PL.
- Se corrigió la selección de ubicación en el mapa para que el clic marque coordenadas.
- Se agregó script SQL completo de base de datos.
