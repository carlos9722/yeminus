# Sistema de órdenes de servicio

Prueba técnica: app para gestionar **clientes**, **técnicos** y **órdenes de mantenimiento**, con login JWT.

Solo usé **PostgreSQL** (el enunciado menciona Oracle también, pero en mi máquina monté todo con Docker).

## Tecnologías

- Backend: .NET 8 Web API + **Dapper** (SQL a mano, sin EF)
- Frontend: Angular 21 (standalone)
- Base de datos: PostgreSQL 16 en Docker
- Auth: JWT

## Requisitos en tu PC

- Docker Desktop
- .NET SDK 8
- Node.js + Angular CLI (`ng`)

## Cómo levantar el proyecto

### 1. Base de datos

En la raíz del repo:

```powershell
docker compose up -d
```

Postgres queda en **localhost:5433** (usuario `root`, clave `123456`, BD `serviceorders_db`).

La primera vez se ejecuta `database/01_schema.sql` y crea las tablas + usuario admin.

### 2. Datos de prueba (opcional)

```powershell
Get-Content "database/02_seed_test_data.sql" | docker exec -i serviceorders-postgres psql -U root -d serviceorders_db
```

Eso borra clientes/técnicos/órdenes y mete datos de ejemplo (el admin no se toca).

### 3. API

```powershell
cd backend
dotnet run --launch-profile http
```

Swagger: http://localhost:5249/swagger

### 4. Frontend

```powershell
cd frontend
ng serve
```

App: http://localhost:4200

El proxy de Angular manda `/api` al puerto 5249.

## Login

| Usuario | Contraseña |
|---------|------------|
| admin   | Admin123!  |

## Qué hace cada módulo

- **Clientes:** CRUD, documento único, teléfonos
- **Técnicos:** CRUD, especialidad
- **Órdenes:** CRUD, al crear quedan en **Pendiente**, en editar puedes cambiar estado
- **Listado de órdenes:** filtros por estado, técnico, cliente, documento y rango de fechas (la consulta se arma en el repo con Dapper)

Los borrados son **soft delete** (`deleted_at`), no se hace `DELETE` físico en las tablas de negocio.

## Arquitectura

No es una arquitectura enorme, pero sí separé responsabilidades para que el código sea fácil de seguir y probar.

### Backend (.NET)

Flujo típico de una petición:

```
Cliente HTTP → Controller → Service → Repository (Dapper + SQL) → PostgreSQL
```

| Carpeta | Para qué la usé |
|---------|------------------|
| `Controllers/` | Reciben HTTP, validan el modelo (`[FromBody]`, Data Annotations) y devuelven 200/400/404, etc. Casi no tienen lógica de negocio. |
| `Services/` | Reglas: teléfono válido, documento duplicado, que el técnico/cliente exista al crear una orden, etc. |
| `Repositories/` | **Todo el SQL** con Dapper. Aquí están los `SELECT`, `INSERT … RETURNING`, soft delete y el filtro dinámico de órdenes (`StringBuilder` + parámetros). |
| `Models/` | DTOs para la API, requests de entrada y entidades que mapean filas de la BD. |
| `Infrastructure/` | Conexión a Postgres (`IDbConnectionFactory`) y constantes como `EntitySqlFilters.ActiveOnly` (`deleted_at IS NULL`). |
| `Helpers/` | Cosas chicas reutilizables: validar teléfono, estados de orden, normalizar filtros. |

**Auth:** `AuthController` + `UserRepository` (login con `crypt` en Postgres). El JWT lo genera `JwtTokenService` y los endpoints de negocio llevan `[Authorize]`.

**Inyección de dependencias:** en `Program.cs` registro interfaces → implementaciones (`IClientService` → `ClientService`, etc.).

No usé Entity Framework a propósito: el enunciado pedía SQL manual y en los filtros de órdenes necesitaba armar la consulta según lo que mande el usuario.

### Frontend (Angular)

Organización por **features** + carpeta **core** compartida:

```
app/
├── core/           → lo que usan varios módulos
├── features/       → pantallas por dominio (auth, clients, technicians, orders)
├── layout/         → shell con menú y navbar
└── app.routes.ts   → rutas
```

| Carpeta | Para qué la usé |
|---------|------------------|
| `core/services/` | Llamadas HTTP (`ClientsService`, `OrdersService`, …). Solo hablan con `/api/...`. |
| `core/models/` | Interfaces TypeScript (tipado de lo que devuelve la API). |
| `core/guards/` | `authGuard` protege rutas internas; `guestGuard` evita volver al login si ya hay sesión. |
| `core/interceptors/` | `auth.interceptor` agrega el `Bearer` en cada request. |
| `core/validators/` y `core/utils/` | Validaciones y formatos del formulario de clientes (documento con puntos, +57, etc.). |
| `features/*/` | Componentes standalone: listado + formulario por módulo. |
| `layout/main-layout/` | Sidebar + navbar + `<router-outlet>` para las pantallas hijas. |

**Rutas:** `/login` va suelta. Lo demás va dentro de `MainLayoutComponent` con `authGuard`. Ejemplo: `/clients`, `/clients/new`, `/orders` con filtros en el listado.

**Estado:** uso signals en varios componentes (`signal`, `computed` donde aplica) para loading, errores y listas. Los formularios son **Reactive Forms**.

**Comunicación con la API:** en desarrollo el `proxy.conf.json` redirige `/api` a `localhost:5249`, así el frontend no hardcodea la URL del backend.

### Cómo se conectan

1. Angular llama `GET /api/orders?status=Pendiente&...`
2. El interceptor manda el JWT
3. `OrdersController` → `ServiceOrderService` → `ServiceOrderRepository` ejecuta el SQL
4. Respuesta JSON en camelCase → la pinta `OrderListComponent`

## Estructura (resumida)

```
prueba tecnica/
├── backend/           → API .NET
├── frontend/          → Angular
├── database/
│   ├── 01_schema.sql
│   └── 02_seed_test_data.sql
├── docker-compose.yml
└── docs/              → notas por partes del desarrollo
```

## Notas

- Si cambias el SQL del esquema y la BD ya existía: `docker compose down -v` y vuelve a subir el contenedor.
- Si `dotnet build` falla porque el `.dll` está bloqueado, detén la API antes de compilar.

