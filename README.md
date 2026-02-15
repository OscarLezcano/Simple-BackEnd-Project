# WebApi Básica con Autenticación - .NET 10

Buenas, este es un proyecto de WebApi básico con autenticación hecho con .NET 10, excelente para utiliza como base en sus proyectos

# Configuración del Proyecto

## 1. Descargar el repositorio

Si tienes la URL del repositorio, ejecuta en tu terminal:

```bash
git clone [URL_DEL_REPOSITORIO]
cd [NOMBRE_DEL_PROYECTO]
```

Si ya lo tienes descargado, simplemente navega a la carpeta raíz del proyecto (donde está el archivo `Program.cs`).

---

## 2. Crear los archivos de configuración

En la raíz del proyecto crea los siguientes archivos:

- `appsettings.json`
- `appsettings.Development.json`

---

## 3. Agregar la configuración

Copia y pega el siguiente contenido en ambos archivos (`appsettings.json` y `appsettings.Development.json`):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "AllowedHosts": "*",

  "ConnectionStrings": {
    "DefaultConnection": "[YOUR_CONNECTION_STRING_HERE]"
  },

  "Jwt": {
    "Key": "[YOUR_SUPER_SECRET_KEY_HERE]"
  }
}
```

---

## 4. Completar los valores

### ConnectionStrings:DefaultConnection

*IMPORTANTE: EL PROYECTO ESTA CONFIGURADO PARA FUNCIONAR CON `PostgreSQL` 
USTED PUEDE UTILIZAR OTRO PERO ESO REQUIRE UNA CONFIGURACIÓN EXTRA

Reemplaza `[YOUR_CONNECTION_STRING_HERE]` con tu cadena de conexión real.
Ejemplo con PostgreSQL:

```json
"DefaultConnection": "Host=localhost;Port=5432;Database=MiBaseDeDatos;Username=postgres;Password=TuPassword;SSL Mode=Require;Trust Server Certificate=true;"
```

#### ¿Donde consigo esto?
Puedes visitar https://neon.com/ o https://supabase.com e invertigar

---

### Jwt:Key

Reemplaza `[YOUR_SUPER_SECRET_KEY_HERE]` con una clave segura de al menos 32 caracteres.

Ejemplo:

```json
"Key": "ESTA_ES_UNA_CLAVE_SUPER_SECRETA_DE_MINIMO_32_CARACTERES"
```

#### ¿Donde consigo la clave?
Vista https://jwtsecrets.com/ y coloca el `Secret Length` en 128 bits dale click en `Generate`

---

## 5. Ejecutar el proyecto
Ejecuta el comando `dotnet restore`(solo una vez) para instalar las "dependencias", luego puedes ejecutar el proyecto con el comando `dotnet run`

Si todo sale bien, al entrar en el siguiente enlace http://localhost:5125/ deberías de poder visualizar algo por el estilo
<img src="./IMGs/Pasted image 20260215152924.png" alt="Descripción de la imagen">

***

## 6. ULTIMO PASO: Configurar la base de datos

### Configurar Roles

Para este proyecto funciones correctamente es necesario realizar una pequeña configuración en tu base de datos.

- En la tabla de `Roles` deberás añadir lo siguiente:
![[Pasted image 20260215153320.png]]

### Creación de administrador

#### Primero crearemos un usuario común:
1. Para eso regresa al swagger y sigue los pasos para registrar un usuario
 ![[Pasted image 20260215153704.png]]
![[Pasted image 20260215153820.png]]
![[Pasted image 20260215153852.png]]

2. Volvemos a la Base Datos y cambiamos su `RolId` a `0` (el cual es el id del admin, si lo configuraste como se indica en este tutorial)  

# Guía para crear un controlador en el proyecto BackEnd
Esta guía explica cómo crear un controlador en este proyecto, incluyendo las mejores prácticas y "mañas" para organizar el código.

## Estructura de carpetas

- **Controllers/**: Aquí van los controladores. Puedes crear subcarpetas para organizar por área (ejemplo: `Admin/`, `Auth/`).

- **Models/Request/**: Contiene los DTOs (Data Transfer Objects) para las solicitudes. Ejemplo: `LoginRequestDto`, `RegisterRequestDto`, `PhoneNumberRequestDto`. Aquí defines las propiedades que esperas recibir del cliente.

- **Models/Response/**: Contiene los DTOs para las respuestas. Ejemplo: `ApiResponseDto`, `UserResponseDto`. Aquí defines cómo se estructura la respuesta que envías al cliente.

- **Models/Mappings/**: Aquí van las clases que transforman entre entidades y DTOs. Ejemplo: `UserMapper`. Usar mappers ayuda a separar la lógica de transformación de datos.

- **Models/Constants/**: Aquí defines constantes como mensajes de error (`ApplicationError`) y mensajes de éxito (`ApplicationMessage`). Útil para centralizar textos y evitar duplicidad.

## Creación de Dtos

Asumiendo que ya sabes sobre las migraciones, las relaciones entre `objetos -> tablas` dentro de  .NET y ademas de que es un Dto.

Vamos a comenzar con la creación de estos, primero que nada dentro de este proyectos definimos dos tipos de Dtos:

- Los  que se usan para recibir peticiones y van en la carpeta `Models/Request` (los llamo RequestDtos)

- Los que se usan para las respuestas y van en la carpeta `Models/Response` (los llamo ResponseDtos)
#### Los  RequestDtos
Son aquellos que llegan dentro de una petición, sus objetivos son:

- Recibir solo lo necesario
- Validar datos
- Nunca exponer propiedades internas (Id generado, navegación, flags internos, etc.)

por ejemplo `LoginRequestDto`:

```C#
using System.ComponentModel.DataAnnotations;
using BackEnd.Models.Constants;
namespace BackEnd.Models.Request.Auth;
public class LoginRequestDto
{
	[Required(ErrorMessage = "El email es obligario")]
	[EmailAddress(ErrorMessage = "El email es invalido")]
	public string Email { get; set; } = null!;
	
	[Required(ErrorMessage = "La contraseña es obligaria")]
	public string Password { get; set; } = null!;
}
```

Se caracterizan por poseer `DataAnnotations` que en .NET es una forma de realizar validaciones semejantes a los if.

Ejemplo:

 ```C#
	[Required(ErrorMessage = "La contraseña es obligaria")]
	public string Password { get; set; } = null!;
 ```
 
 Aquí ErrorMessage contiene un string hardcodeado  sin embargo en nuestro código nosotros guardamos estos mensajes dentro de constantes como se aprecia en la imagen
 
 ![[Pasted image 20260215162420.png]]

Y los usamos de la siguiente manera *(recomiendo encarecidamente revisar esos archivos para no perderse)*

```C#
using System.ComponentModel.DataAnnotations;
using BackEnd.Models.Constants; // Primero las importamos :)
namespace BackEnd.Models.Request.Auth;
public class LoginRequestDto
{
	[Required(ErrorMessage = ApplicationError.RequiredField.EmailRequired)]
	[EmailAddress(ErrorMessage =
		 ApplicationError.ValidationError.InvalidEmail)]
	public string Email { get; set; } = null!;
	
	[Required(ErrorMessage = ApplicationError.RequiredField.PasswordRequired)]
	public string Password { get; set; } = null!;
}
```
#### Los ResponseDtos
Primero tomar en cuenta como se representa `User` en nuestra tabla
```C#
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace BackEnd.Models;
// Esto sirve para ningun email se repita
[Index(nameof(Email), IsUnique = true)]
public class User
{
	// Esta anotacion va encima de nuestro ID
	[Key]
	public Guid Id { get; set; } = Guid.NewGuid();
	
	// null! le asegura a C# que esos nunca seran nulos
	public string Name { get; set; } = null!;
	public string LastName { get; set; } = null!;
	public string Email { get; set; } = null!;
	public string PasswordHash { get; set; } = null!;
	public int RoleId { get; set; } = 1; //User por defecto
	
	// Relaciones con otras tablas
	// Para relaciones con otras tablas usamos virtual
	public virtual List<PhoneNumber>? PhoneNumbers { get; set; }
	public virtual Role? Role { get; set; }
}
```

Los ResponseDtos son los que se usan para las respuestas, sus objetivos son:

- ir en return en controllers
- ser las respuestas de servicios
- evitar exponer entidades completas, por ejemplo el ID aunque aquí si que lo mostramos :)

por ejemplo `UserResponseDto`:

```C#
namespace BackEnd.Models.Response.User;
public class UserResponseDto
{
	public Guid Id { get; set; }
	public string Name { get; set; } = null!;
	public string LastName { get; set; } = null!;
	public string Email { get; set; } = null!;
	public List<string> PhoneNumbers { get; set; } = [];
	public string RoleName { get; set; } = null!;
}
```

 Aquí simplemente van los datos que queres expones de tu base datos, piensa en ellas como en una view.

Ahora para que estas "views" funcionen se deben de mapear(ojo solo se mapean los `ResponseDtos`) para mapearlos deber de la carpeta `Models/Mappings` puedes seguir el siguiente ejemplo:

```C#
using BackEnd.Models.Response.User;
namespace BackEnd.Models.Mappings;
public class UserMapper : AutoMapper.Profile // Extender de AutoMapper.Profile
{
	public UserMapper()
	{
		// Tenemos dos posibles casos
		// Caso basico: Propiedades con el mismo nombre son mapeados 
		// autmaticamente
		
		// Caso avanzado: Los que lleven el mismo nombre deberan ser mapeados
		// manualmente como Role
		CreateMap<User, UserResponseDto>()
			.ForMember(
				dest => dest.RoleName,
				opt => opt.MapFrom(src => src.Role!.Name)) 
		
		// Nola el ! frente a src.Role y PhoneNumbers sirven para decirle
		// al compilador "Estos valores jamas seran nulos silencio >:V"
	}
}
```

### Ahora si Controladores

Primero revisaremos su estructura

```C#
// Todas estas importaciones son casi siempre necesarias
using Microsoft.AspNetCore.Mvc;
using BackEnd.Models.Response.User;
using BackEnd.Models.Response;
using BackEnd.Context;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using BackEnd.Models.Constants;

// namespace omnipresente en todas las clases de C#
namespace BackEnd.Controllers;

// ruta del controlador
[Route("api/example")] 
[ApiController]
public class ExampleController(AppDbContext context, IMapper mapper) 
	: ControllerBase
{
private readonly AppDbContext _context = context;
private readonly IMapper _mapper = mapper;

// Como ejemplo haremos un get -----------------------------------------
[HttpGet("ejemplo-endpoint")] 
// El endpoint tambien puede estar vacio para acceder directamente a
// api/example/ en lugar de api/example/ejemplo-endpoint
// ---------------------------------------------------------------------

[Authorize] // Si necesita estar atenticado
[Authorize(Roles = "Admin")] // Si necesita estar autenticado y ser admin

// Envolvemos todo en ApiResponseDto para mantener la consistencia de los datos aunque esto es opcional
public async Task<ActionResult<ApiResponseDto>> GetUsers()
{
	var users = await _context.Users
		.Include(u => u.Role)
		.Include(u => u.PhoneNumbers)
		.ToListAsync();
		
	// Suponiendo que hubo un error
	if (users == null) {
		// Normalmente utilizo las contanstes para eviar el mensaje, 
		// pero ustedes eligen
		
		// string errorMessage = ApplicationError.NotFoundError.UsersNotFound;
		string errorMessage = "No se encontraron usuarios";
		return NotFound(new ApiResponseDto
			{
				Success = false,
				Message = errorMessage,
				Errors = new { User = new[] { errorMessage } }
			});
	}	
		
	return Ok(new ApiResponseDto
	{
		Success = true,
		Message = ApplicationMessages.Success.UsersRetrieved,
		// Utilizamos nuestro mapper 
		Data = _mapper.Map<List<UserResponseDto>>(users)
	});
}
```

Esta es la clase `ApiResponseDto` para que vean 

```C#
namespace BackEnd.Models.Response;
public class ApiResponseDto
{
	public bool Success { get; set; }
	public string Message { get; set; } = string.Empty;
	public object? Data { get; set; }
	public object? Errors { get; set; }
}
```

Aquí solo utilizamos un ResponseDto, ahora vemos otro con un RequestDto como ejemplo (pero ahora solo la función el resto es prácticamente lo mismo)

```C#
// Indica que este endpoint responde a peticiones HTTP POST en la ruta "login"
[HttpPost("login")]
public async Task<IActionResult> Login(LoginRequestDto request)
{
    // Busca en la base de datos un usuario cuyo Email coincida con el enviado 
    // en el request.
    
    // Include(u => u.PhoneNumbers) carga también los números de teléfono 
    // relacionados
    
    // Include(u => u.Role) carga el rol del usuario.
    var user = await _context.Users
        .Include(u => u.PhoneNumbers)
        .Include(u => u.Role)
        .FirstOrDefaultAsync(u => u.Email == request.Email);

    // Si el usuario no existe O la contraseña no coincide:
    // BCrypt.Verify compara la contraseña enviada con el hash guardado en la 
    // base de datos.
    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, 
	    user.PasswordHash))
    {
        // Obtiene el mensaje de error definido en constantes de la aplicación
        string errorMessage = 
	        ApplicationError.ValidationError.InvalidCredentials;

        // Devuelve un 400 BadRequest con una respuesta estructurada
        return BadRequest(new ApiResponseDto
        {
            Success = false,
            Message = errorMessage,
            Errors = new { Authentication = new[] { errorMessage } }
        });
    }
	
	//....codigo sin importancia....

    // Guarda el token en una cookie llamada "current_user"
    Response.Cookies.Append("current_user", token, cookieOptions);

    // Devuelve una respuesta 200 OK indicando que el login fue exitoso
    return Ok(new ApiResponseDto
    {
        Success = true,
        Message = ApplicationMessages.Authentication.LoginSuccessful,
        
        // Se mapea la entidad User a un DTO para no exponer datos sensibles
        Data = _mapper.Map<UserResponseDto>(user)
    });
}

```
