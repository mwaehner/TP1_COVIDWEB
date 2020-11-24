# TP1_ARQWEB
Proyecto Visual Studio 19 del TP1 de Arquitectura App Web

Si se compila el proyecto y se lo inicia aparece la pantalla de logueo al sitio.
Para poder acceder a pantalla "home" es necesario registrarse al sitio. para esto se debe contar con una base de datos de usuarios.
Los pasos para crear database de usuarios:

1 - en appsettings.json hay que modificar código para conectarse a server activo. Para esto levantar alguno con sql management studio o azure studio, por ejemplo un sqlexpress, y
 colocar el nombre del server en la linea 11, esto es:
	  
   
   
    "MvcLocationContext": "Server= <nombre server>;Database=<nombre database a crear> ;Trusted_Connection=True;MultipleActiveResultSets=true"
  

por ejemplo:	

	
    
    "MvcLocationContext": "Server=(localdb)\\mssqllocaldb;Database=master5;Trusted_Connection=True;MultipleActiveResultSets=true"
  

2 - seleccionar el nombre del proyecto (al inicio del arbol que despliega visual studio, derecha), y en la pestaña "herramientas" (arriba) ir a "administrador de paquetes nuget"
y seleccionar "consola de administrador de paquetes"

3 - en la consola desplegada (abajo) tipear (en orden):

  	Add-Migration "Initial-Create"
    Update-Database

 

4 - ingresar email y password en pantalla de registro

5 - si están conectados a una base de datos podrán acceder al home

6 - salir y volver a loguearse con mismo mail y password, el sitio los tiene que tener registrados.

7 - para que se pueda mostrar los códigos QR, se debe ir a Tools/NuGet Package Manager/Manage NuGet Packages for Solution
y en Browse buscar el paquete QRCoder de Raffael Herrmann e instalarlo. También instalar el paquete Newtonsoft.json.

8 - para acceder a pantalla administrador (credenciales hardcodeadas):
	usermail: admin@gmail.com
	password: adminadmin

9 - una vez logueado como admin buscar botón adiministrador al lado de los demás

10 - Para la api: buscar "webapi" en el buscador de paquetes NuGet e instalar los primeros 4.

11 - Para testear api location ingresar url: 
	
	https://localhost:<puerto>/api/location/<id locacion requerida>
