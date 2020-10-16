# TP1_ARQWEB
Proyecto Visual Studio 19 del TP1 de Arquitectura App Web

Si se compila el proyecto y se lo inicia aparece la pantalla de logueo al sitio.
Para poder acceder a pantalla "home" es necesario registrarse al sitio. para esto se debe contar con una base de datos de usuarios.
Los pasos para crear database de usuarios:

1 - en appsettings.json hay que modificar código para conectarse a server activo. Para esto levantar alguno con sql management studio o azure studio, por ejemplo un sqlexpress, y
 colocar el nombre del server en la linea 10, esto es:
	  
    "ConnectionStrings": "Server= <nombre server> ;Database= <nombre database a crear> ;Trusted_Connection=True;MultipleActiveResultSets=true"

por ejemplo:	
	
    "ConnectionStrings": "Server= localhost\\SQLEXPRESS;Database=TP1_ARQWEB_DB2;Trusted_Connection=True;MultipleActiveResultSets=true"

2 - seleccionar el nombre del proyecto (al inicio del arbol que despliega visual studio, derecha), y en la pestaña "herramientas" (arriba) ir a "administrador de paquetes nuget"
y seleccionar "consola de administrador de paquetes"

3 - en la consola desplegada (abajo) tipear (en orden):

  	Add-Migration "Initial-Create"
    Update-Database

Cambio de matías: como ahora hay dos contextos (TP1_ARQWEBdbContext y MvcLocationContext), hay que hacer lo anterior para cada uno.

    Add-Migration "Initial-Create" -Context TP1_ARQWEBdbContext
    Add-Migration "Initial-Create2" -Context MvcLocationContext
    update-database -Context TP1_ARQWEBdbContext
    update-database -Context MvcLocationContext

4 - ingresar email y password en pantalla de registro

5 - si están conectados a una base de datos podrán acceder al home

6 - salir y volver a loguearse con mismo mail y password, el sitio los tiene que tener registrados.

7 - para que se pueda mostrar los códigos QR, se debe ir a Tools/NuGet Package Manager/Manage NuGet Packages for Solution
y en Browse buscar el paquete QRCoder de Raffael Herrmann e instalarlo.


