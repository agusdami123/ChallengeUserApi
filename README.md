# Usuarios ABM (Vue + .NET 8)

Gestión de usuarios con **ABM** completo: crear, listar, editar y eliminar.
Frontend en **Vue 3 + PrimeVue (tema Lara)** y backend **.NET 8 + SQLite** (con opción a SQL Server).

## Requisitos

* **.NET 8 SDK**
* **Node.js 18+** (con npm)
* (Opcional) **SQL Server** si no querés usar SQLite

## Pasos rápidos

1. **Backend**

   ```bash
   cd backend
   dotnet restore
   dotnet run
   ```

   API: [http://localhost:5099](http://localhost:5099)
   Swagger: [http://localhost:5099/swagger](http://localhost:5099/swagger)

2. **Frontend**

   ```bash
   cd ../frontend
   npm install
   npm run dev
   ```

   Web: [http://localhost:5173](http://localhost:5173)

---

## Qué incluye (Checklist del enunciado)

* **ABM vía API** (`GET/POST/PUT/DELETE /api/usuarios`) consumido con **axios** desde Vue.
* **Tipo** con `<Select>` (PrimeVue Dropdown) con valores: **Administrador, Cliente, Agente**.
* **Correo** validado: debe tener `@` y terminar con `.com` o TLD de **2+** letras.
* **Teléfono**: **sólo dígitos** (7 a 15). El input limpia todo lo que no sea número.
* **Correo único**: índice único en DB → la API responde **409** si ya existe.
* **Activo**: booleano manejado con checkbox/switch en el frontend.
* **Extras**: paginado fijo en **10**, **filtros** por **Tipo** y **Estado**, **modo oscuro** con toggle y persistencia en `localStorage`, toasts **una a la vez**.

---

## Modelo de datos

```json
{
  "id": 1,
  "descripcion": "Juan Perez",
  "tipo": "Administrador", 
  "correoElectronico": "juan@example.com",
  "telefono": "1122334455",
  "activo": true
}
```

---

## API (REST)

Base URL: `http://localhost:5099/api/usuarios`

| Método | Ruta                 | Descripción | Códigos         |
| -----: | -------------------- | ----------- | --------------- |
|    GET | `/api/usuarios`      | Lista todos | 200             |
|    GET | `/api/usuarios/{id}` | Obtiene uno | 200/404         |
|   POST | `/api/usuarios`      | Crea        | 201/400/409     |
|    PUT | `/api/usuarios/{id}` | Actualiza   | 200/400/404/409 |
| DELETE | `/api/usuarios/{id}` | Elimina     | 204/404         |

### Ejemplos `curl`

```bash
# Crear
curl -X POST http://localhost:5099/api/usuarios \
  -H "Content-Type: application/json" \
  -d '{"descripcion":"Ana","tipo":"Cliente","correoElectronico":"ana@dominio.com","telefono":"11223344","activo":true}'

# Listar
curl http://localhost:5099/api/usuarios

# Actualizar (id=1)
curl -X PUT http://localhost:5099/api/usuarios/1 \
  -H "Content-Type: application/json" \
  -d '{"id":1,"descripcion":"Ana Gomez","tipo":"Cliente","correoElectronico":"ana@dominio.com","telefono":"11998877","activo":false}'

# Eliminar (id=1)
curl -X DELETE http://localhost:5099/api/usuarios/1
```

---

## Base de datos

* **Por defecto** usa **SQLite** en `backend/usuarios.db`.
* El esquema se crea automáticamente con `EnsureCreated()` al levantar.

### Cambiar a SQL Server (opcional)

1. Instalar paquete:

```bash
cd backend
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

2. En `Program.cs`, reemplazar `UseSqlite(...)` por:

```csharp
options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
```


3. Reiniciar el backend. (Para producción, migraciones EF es lo recomendado.)

---

## Frontend

* **Vue 3 + Vite + PrimeVue (Lara)**.
* **Modo oscuro**: toggle que alterna clase `p-dark` en `<html>` y se guarda en `localStorage`.
* **Tabla**: PrimeVue DataTable, **paginado 10**, filtros por **Tipo** y **Estado**.

Scripts:

```bash
npm run dev     # desarrollo
npm run build   # build de producción
npm run preview # server de vista previa del build
```


## Estructura

```
usuarios-abm-starter/
├─ backend/
│  ├─ Program.cs
│  ├─ Usuarios.Api.csproj
│  └─ appsettings.json
└─ frontend/
   ├─ index.html
   ├─ vite.config.js
   └─ src/
      ├─ main.js
      ├─ App.vue
      └─ UsuarioCrud.vue
```

