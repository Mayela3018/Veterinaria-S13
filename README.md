# 🐾 Veterinaria API — Web API REST en capas

API REST para una **veterinaria** que gestiona el registro de **dueños** y sus **mascotas**.  
Está construida con **ASP.NET Core (.NET 10)** usando **arquitectura en capas**, **DTOs**, **AutoMapper** y **Entity Framework Core** con enfoque **Database First** sobre **SQL Server**.

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-10-6C3483)
![AutoMapper](https://img.shields.io/badge/AutoMapper-DTOs-BE1E2D)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?logo=swagger&logoColor=black)

---

## 📌 Problema que resuelve

Una clínica veterinaria necesita llevar el registro de sus clientes (dueños) y de las mascotas que atiende, evitando datos duplicados o inconsistentes. Esta API permite **registrar, consultar, actualizar y eliminar** dueños y mascotas, y aplica **reglas de negocio** que protegen la integridad de la información.

---

## 🏗️ Arquitectura en capas

La solución tiene **4 proyectos** con referencias en cadena:

```
Veterinaria.Api  →  Veterinaria.Negocio  →  Veterinaria.Datos  →  Veterinaria.Entidades
```

| Capa | Proyecto | Responsabilidad |
|---|---|---|
| **API** | `Veterinaria.Api` | Controladores, `Program.cs`, inyección de dependencias y Swagger. Solo conoce servicios y DTOs. |
| **Negocio** | `Veterinaria.Negocio` | Servicios con sus interfaces, reglas de negocio, `MappingProfile` y `ReglaNegocioException`. |
| **Datos** | `Veterinaria.Datos` | `VeterinariaContext` (EF Core) y repositorios con sus interfaces. Es la única capa que usa el DbContext. |
| **Entidades** | `Veterinaria.Entidades` | Entidades generadas por scaffold y DTOs de lectura y entrada. |

```
Veterinaria/
├── VeterinariaDB.sql
├── Veterinaria.Entidades/
│   ├── Dueno.cs
│   ├── Mascota.cs
│   └── Dtos/
├── Veterinaria.Datos/
│   ├── VeterinariaContext.cs
│   └── Repositorios (IDuenoRepository, DuenoRepository, IMascotaRepository, MascotaRepository)
├── Veterinaria.Negocio/
│   ├── Excepciones/ReglaNegocioException.cs
│   ├── Mapeos/MappingProfile.cs
│   └── Servicios/ (IDuenoService, DuenoService, IMascotaService, MascotaService)
└── Veterinaria.Api/
    ├── Controllers/ (DuenosController, MascotasController)
    └── Program.cs
```

---

## 🗃️ Modelo de datos

Relación **1:N**: un dueño puede tener muchas mascotas y cada mascota pertenece a un solo dueño.

```
Duenos (1) ──────── (N) Mascotas
```

| Dueno | Mascota |
|---|---|
| `Id` (PK) | `Id` (PK) |
| `Nombres` | `Nombre` |
| `Dni` (único, 8 dígitos) | `Especie` |
| `Telefono` | `Raza` |
| `Email` | `Edad` (0 a 40) |
| | `Peso` (> 0) |
| | `DuenoId` (FK → Duenos) |

---

## ⚖️ Reglas de negocio

Se implementan en la capa de **Negocio**, lanzan `ReglaNegocioException` y el controlador las convierte en **409 Conflict**:

1. No se puede registrar ni actualizar un dueño con un **DNI que ya existe**.
2. No se puede **eliminar un dueño que tiene mascotas**.
3. Una mascota debe pertenecer a un **dueño que exista**.
4. Un dueño no puede tener **dos mascotas con el mismo nombre y especie**.

---

## 🔄 DTOs y AutoMapper

La API **nunca expone entidades**: recibe y devuelve solo DTOs. Así se evita el ciclo de JSON (sin `[JsonIgnore]`) y se controla qué datos entran y salen.

| Mapeo | Origen → Destino | Configuración especial |
|---|---|---|
| `Dueno → DuenoDto` | Entidad → DTO de lectura | `CantidadMascotas` toma `Mascotas.Count` |
| `DuenoCrearDto → Dueno` | DTO de entrada → Entidad | Ninguna |
| `DuenoActualizarDto → Dueno` | DTO de entrada → Entidad existente | Se usa `Map(origen, destino)` |
| `Mascota → MascotaDto` | Entidad → DTO de lectura | `DuenoNombre` toma `Dueno.Nombres` |
| `MascotaCrearDto → Mascota` | DTO de entrada → Entidad | Ninguna |
| `MascotaActualizarDto → Mascota` | DTO de entrada → Entidad existente | Se usa `Map(origen, destino)` |

Los DTOs de entrada se validan con **Data Annotations** (`[Required]`, `[StringLength]`, `[Range]` y `[RegularExpression]` para el DNI).

---

## 🌐 Endpoints

### Dueños — `api/duenos`
| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| GET | `/api/duenos` | Lista dueños (filtros por query string) | 200 |
| GET | `/api/duenos/{id}` | Obtiene un dueño | 200, 404 |
| GET | `/api/duenos/{id}/mascotas` | Mascotas de un dueño (ruta anidada) | 200, 404 |
| POST | `/api/duenos` | Registra un dueño | 201, 400, 409 |
| PUT | `/api/duenos/{id}` | Actualiza un dueño | 204, 400, 404, 409 |
| DELETE | `/api/duenos/{id}` | Elimina un dueño | 204, 404, 409 |

### Mascotas — `api/mascotas`
| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| GET | `/api/mascotas` | Lista mascotas (filtros por query string) | 200 |
| GET | `/api/mascotas/{id}` | Obtiene una mascota | 200, 404 |
| POST | `/api/mascotas` | Registra una mascota | 201, 400, 409 |
| PUT | `/api/mascotas/{id}` | Actualiza una mascota | 204, 400, 404, 409 |
| DELETE | `/api/mascotas/{id}` | Elimina una mascota | 204, 404 |

### Códigos HTTP utilizados
| Código | Significado |
|---|---|
| **200** OK | Consulta exitosa |
| **201** Created | Recurso creado con `CreatedAtAction` |
| **204** No Content | PUT o DELETE exitoso |
| **400** Bad Request | Datos inválidos en el DTO |
| **404** Not Found | El recurso no existe |
| **409** Conflict | Regla de negocio incumplida |

---

## 🚀 Cómo ejecutar el proyecto

### Requisitos
- .NET 10 SDK
- SQL Server (por ejemplo, SQL Server Express) y SSMS
- Visual Studio 2022/2026 o VS Code

### Pasos
1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/Mayela3018/Veterinaria-S13.git
   ```
2. **Crear la base de datos**: abrir `VeterinariaDB.sql` en SSMS y ejecutarlo. Crea la base `VeterinariaDB` con datos de ejemplo.
3. **Configurar la cadena de conexión**: crear el archivo `Veterinaria.Api/appsettings.Development.json`, que no se sube al repositorio:
   ```json
   {
     "ConnectionStrings": {
       "VeterinariaDb": "Server=TU_SERVIDOR\\SQLEXPRESS;Database=VeterinariaDB;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```
4. **Ejecutar la API**
   ```bash
   dotnet run --project Veterinaria.Api
   ```
5. **Probar en Swagger**: abrir `https://localhost:<puerto>/swagger`.

---

## 🧰 Tecnologías

- ASP.NET Core Web API (.NET 10) con controladores
- Entity Framework Core 10 (SQL Server, Database First)
- AutoMapper
- Swagger / OpenAPI
- Inyección de dependencias (`AddScoped`)
- LINQ y programación asíncrona (`async/await`)

---

## 👩‍💻 Autora

**Mayela Ticona Mamani**  
Estudiante de Diseño y Desarrollo de Software 
