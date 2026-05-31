# Sistema de órdenes de servicio

Prueba técnica: app para gestionar **clientes**, **técnicos** y **órdenes de mantenimiento**, con login JWT.

Solo usé **PostgreSQL** (el enunciado menciona Oracle también, pero en mi máquina monté todo con Docker).

## Tecnologías

- Backend: .NET 8 Web API + **Dapper** (SQL a mano, sin EF)
- Frontend: Angular 21 (standalone)
- Base de datos: PostgreSQL 16 en Docker
- Auth: JWT

## Requisitos en tu PC

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (LTS recomendado)
- Angular CLI: `npm install -g @angular/cli`

Comprobar que todo esté instalado:

```powershell
docker --version
dotnet --version
node --version
npm --version
ng version
```

## Instalación (primera vez)

Clona o descomprime el proyecto y, en la **raíz del repo**, ejecuta:

### 1. Dependencias del backend

```powershell
cd backend
dotnet restore
cd ..
```

### 2. Dependencias del frontend

```powershell
cd frontend
npm install
cd ..
```

> Si `ng` no se reconoce: `npm install -g @angular/cli` y vuelve a abrir la terminal.

### 3. Base de datos (Docker)

Desde la raíz (donde está `docker-compose.yml`):

```powershell
docker compose up -d
```

La primera vez descarga la imagen de Postgres y ejecuta `database/01_schema.sql` (tablas + usuario **admin**).

### 4. Datos de prueba (opcional)

```powershell
Get-Content "database/02_seed_test_data.sql" | docker exec -i serviceorders-postgres psql -U root -d serviceorders_db
```

---

## Cómo levantar el proyecto (cada vez que desarrollas)

Abre **dos terminales** (API y frontend).

### Terminal 1 — API

```powershell
cd backend
dotnet run --launch-profile http
```

- Swagger: http://localhost:5249/swagger  
- Si falla el build porque el `.dll` está bloqueado, detén este proceso y vuelve a ejecutar.

### Terminal 2 — Frontend

```powershell
cd frontend
ng serve
```

- App: http://localhost:4200  
- El proxy (`proxy.conf.json`) envía `/api` → `http://localhost:5249`

### Base de datos

Si Docker no está corriendo:

```powershell
docker compose up -d
```

Postgres: **localhost:5433** — usuario `root`, clave `123456`, BD `serviceorders_db`.

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

| Parte | Nombre / estilo |
|-------|------------------|
| **Backend** | **Arquitectura en capas** (N-Capas) + **API REST** + patrón **Repository** |
| **Frontend** | **SPA** Angular con organización **por features** + componentes **standalone** |

No es microservicios ni Clean Architecture completa; es una estructura clara para una prueba técnica de tamaño medio.

### Backend (.NET) — Capas + Repository

**Nombre:** arquitectura en **capas** (presentación → negocio → datos) sobre una **Web API REST**.

| Capa | Carpeta | Rol |
|------|---------|-----|
| Presentación | `Controllers/` | HTTP, rutas, códigos de respuesta |
| Negocio | `Services/` | Reglas y validaciones |
| Datos | `Repositories/` | SQL con Dapper (patrón **Repository**) |
| Transversal | `Infrastructure/`, `Helpers/` | Conexión BD, utilidades |
| Modelos | `Models/` | DTOs, requests y entidades que mapean tablas |

Flujo típico de una petición:

```
Cliente HTTP → Controller → Service → Repository (Dapper + SQL) → PostgreSQL
```

**Auth:** `AuthController` + `UserRepository` (login con `crypt` en Postgres). El JWT lo genera `JwtTokenService` y los endpoints de negocio llevan `[Authorize]`.

**Inyección de dependencias:** en `Program.cs` registro interfaces → implementaciones (`IClientService` → `ClientService`, etc.).

No usé Entity Framework a propósito: el enunciado pedía SQL manual y en los filtros de órdenes necesitaba armar la consulta según lo que mande el usuario.

### Frontend (Angular) — SPA por features

**Nombre:** **Single Page Application (SPA)** con carpetas **por dominio (feature-based)** y componentes **standalone** (sin `NgModule` por módulo).

| Zona | Carpeta | Rol |
|------|---------|-----|
| Núcleo compartido | `core/` | Servicios HTTP, guards, interceptor JWT, modelos |
| Pantallas | `features/` | Un feature por dominio (auth, clients, technicians, orders) |
| Shell | `layout/` | Layout con menú y navbar |

Estructura:

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