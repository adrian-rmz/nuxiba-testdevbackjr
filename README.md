# EVALUACIÓN TÉCNICA NUXIBA - DESARROLLADOR JR

**Nombre:** Adrián Alejandro Ramírez Cruz

## Descripción

Este proyecto incluye:

- **Ejercicio 1:** API RESTful con ASP.NET Core, Entity Framework Core y SQL Server para manejar registros de login/logout.
- **Ejercicio 2:** Consultas SQL para calcular tiempos de login.
- **Ejercicio 3:** Endpoint para descargar un CSV con horas trabajadas por usuario.

---

## Tecnologías

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server en Docker
- xUnit

---

## 1. Levantar SQL Server

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
```

Si el contenedor ya existe:

```bash
docker start sqlserver
```

---

## 2. Crear la base de datos

Conectarse a SQL Server con:

- **Server:** `localhost,1433`
- **User:** `sa`
- **Password:** `YourStrong!Passw0rd`

Ejecutar:

```sql
IF DB_ID('NuxibaDb') IS NULL
BEGIN
    CREATE DATABASE NuxibaDb;
END;
GO
```

---

## 3. Ejecutar la API

Entrar al proyecto:

```bash
cd NuxibaEvaluation.Api
```

Restaurar paquetes:

```bash
dotnet restore
```

Aplicar migraciones:

```bash
dotnet ef database update
```

Si fuera necesario crear la migración desde cero:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Ejecutar la API:

```bash
dotnet run
```

Abrir Swagger en la URL que muestre la consola, por ejemplo:

```txt
http://localhost:xxxx/swagger
```

---

## 4. Cargar los datos del Excel

Ejecutar el archivo:

```txt
sql/seed-data.sql
```

Verificar con:

```sql
SELECT COUNT(*) AS TotalLogins FROM ccloglogin;
SELECT COUNT(*) AS TotalUsers FROM ccUsers;
SELECT COUNT(*) AS TotalAreas FROM ccRIACat_Areas;
GO
```

Resultado esperado:

- `TotalLogins = 10000`
- `TotalUsers = 137`
- `TotalAreas = 2`

---

## 5. Endpoints

### `GET /logins`

Obtiene todos los registros de login/logout.

### `POST /logins`

Crea un nuevo registro.

Ejemplo:

```json
{
  "userId": 70,
  "extension": 1,
  "tipoMov": 1,
  "fecha": "2024-10-05T08:00:00"
}
```

### `PUT /logins/{id}`

Actualiza un registro existente.

### `DELETE /logins/{id}`

Elimina un registro existente.

### `GET /reports/worked-hours-csv`

Descarga el CSV del ejercicio 3.

---

## 6. Ejercicio 2

Las consultas SQL están en:

```txt
sql/exercise-2-queries.sql
```

Incluyen:

- usuario con más tiempo logueado
- usuario con menos tiempo logueado
- promedio de tiempo logueado por usuario y por mes

---

## 7. Descargar el CSV

### Desde Swagger

1. Ejecutar la API
2. Abrir `/swagger`
3. Buscar `GET /reports/worked-hours-csv`
4. Presionar `Try it out`
5. Presionar `Execute`

### Desde curl

```bash
curl -X GET "http://localhost:xxxx/reports/worked-hours-csv" --output worked-hours-report.csv
```

> Reemplazar `xxxx` por el puerto real de la API.

---

## 8. Pruebas unitarias

Entrar a la carpeta de Tests:

```bash
cd NuxibaEvaluation.Api.Tests
```

Ejecutar:

```bash
dotnet test
```

---

## Notas

- Se agregó una columna `Id` en `ccloglogin` para soportar `PUT` y `DELETE`.
- En `ccRIACat_Areas` había un `IDArea` duplicado en el archivo original, por eso se conservaron solo valores únicos al cargar los datos.
- Para los cálculos de tiempo solo se consideraron sesiones válidas: `login` seguido por `logout`.
