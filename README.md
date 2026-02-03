# Prueba Técnica

API REST desarrollada en **.NET 6** con **Entity Framework Core** y **SQL Server**, cumpliendo todos los requisitos del documento de prueba técnica.

## 🔑 Características implementadas

- ✅ Registro de usuario con validaciones completas
- ✅ Login con correo o username
- ✅ Máximo 3 intentos fallidos → bloqueo automático
- ✅ Solo 1 sesión activa por usuario
- ✅ Registro de sesiones (inicio y cierre)
- ✅ Correo electrónico generado automáticamente
- ✅ Eliminación lógica
- ✅ Roles y permisos
- ✅ Stored Procedure y función en SQL Server
- ✅ Contraseñas hasheadas con BCrypt

## 🛠️ Requisitos

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- SQL Server (Express o superior)
- Visual Studio 2022 (o VS Code + extensiones)
- Postman (para pruebas)

## 🗃️ Base de datos

1. Ejecutar el script `script_db_banco.sql` en SQL Server.
2. Se creará la base de datos **`BancoDb`** con todas las tablas, funciones, SP y datos de prueba.

### Credenciales de prueba
| Campo | Valor |
|------|-------|
| **Usuario** | `AdminRoot123` |
| **Contraseña** | `Passw0rd!` |
| **Rol** | Administrador |

## ▶️ Ejecutar la API

1. Abrir la solución `BancoApi.sln` en Visual Studio 2022.
2. Presionar **F5** o hacer clic en **IIS Express**.
3. La API correrá en `https://localhost:7123` (el puerto puede variar).

## 🧪 Probar con Postman

1. Importar la colección: `BancoApi.postman_collection.json`
2. Asegurarse de que la variable `baseUrl` apunte a tu URL local (ej. `https://localhost:7123`)
3. Endpoints disponibles:

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/auth/register` | Registrar nuevo usuario |
| POST | `/api/auth/login` | Iniciar sesión |
| POST | `/api/auth/logout/{id}` | Cerrar sesión |
| GET | `/api/roles` | Listar roles disponibles |
| GET | `/api/auth` | Listar usuarios |
| DELETE | `/api/auth/{id}` | Eliminar usuario (lógicamente) |

## 📂 Estructura del proyecto
BancoApi/

├── Controllers/ → Endpoints API

├── Models/ → Entidades de base de datos

├── DTOs/ → Objetos de transferencia

├── Services/ → Lógica de negocio

├── Data/ → DbContext y configuración

└── Helpers/ → Validaciones y utilidades

## 📌 Notas importantes

- Las contraseñas **nunca se almacenan en texto plano** (se usan hashes BCrypt).
- La eliminación es **lógica** (columna `Eliminado = true`).
- El SP `sp_RegistrarUsuario` y la función `ValidarIdentificacion` demuestran conocimientos en SQL Server.
- En una implementación real, los endpoints de administración requerirían autenticación JWT.

---
Desarrollado para prueba técnica - Roberto Daza
