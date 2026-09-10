# Hotel Management System

Original apps, with SQL Server in Docker instead of Azure.

## The three apps

1. **Customer web** (`Oblig4Azure`) — ASP.NET MVC  
2. **Front desk** (`HotelFront`) — WPF desktop  
3. **Staff** (`ServiceAppMaui`) — .NET MAUI  

They talk through the web API on http://localhost:5099 and the Docker database.

## Mac

`HotelFront` is WPF and **cannot run on macOS**. That is a .NET/Windows limit, not something this repo can change. On a Mac you can run the customer web and the MAUI staff app.

### 1. Database + customer web

```bash
./start.sh
```

Open http://localhost:5099  
Register a user, or use `guest@hotel.local` / `Password123!`

Leave this terminal running.

### 2. Staff app (MAUI)

New terminal:

```bash
dotnet run --project ServiceAppMaui/ServiceAppMaui.csproj -f net9.0-maccatalyst
```

### 3. Front desk (WPF)

Only on Windows:

```bash
dotnet run --project HotelFront/HotelFront.csproj
```

## Database

Docker SQL Server, connection string in `Oblig4Azure/appsettings.json`.
