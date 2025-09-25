# Backend (.NET 8 Web API)

## Requisitos
- .NET 8 SDK
- (Opcional) SQL Server si cambiás a ese provider

## Ejecutar
```bash
cd backend
dotnet restore
dotnet run
```
La API corre en: **http://localhost:5099** (Swagger: `/swagger`).

Por defecto usa **Sqlite** (archivo `usuarios.db`). Cambiá a SQL Server editando `appsettings.json`:

```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "SqlServer": "Server=localhost;Database=UsuariosDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

> Para dev rápida usamos `EnsureCreated()`. En entornos formales preferí **migraciones** (`dotnet ef migrations add Initial && dotnet ef database update`).

CORS está abierto para dev, así Vue puede llamar a `/api/usuarios`.
