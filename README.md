# Monologer

**Monologer** je C# aplikace na **.NET 9**, která přijímá WebSocket zprávy a ukládá je do **MS SQL Server**. Projekt slouží jako ukázka práce s WebSockety a na  paralelizací.

---

## Předpoklady

net ef
.net 9
MSSQL

## (jen ucitelských pc )
instalace .net ef tools 
```bash
cd Monologer
dotnet new tool-manifest
dotnet tool install --local dotnet-ef --version 9.0.11
```
## Instalace

1. Klonujte repozitář:

```bash
git clone https://github.com/yourusername/Monologer.git
cd Monologer
```

2. Obnovte závislosti:

```bash
dotnet restore
```

3. Nakonfigurujte databázi v `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your ConnectionString here;"
  },
  "WorkerPool": {
    "MaxThreads": 4
  }
}
```

4. Aplikujte migrace:

```bash
dotnet ef database update
```

5. Spusťte aplikaci:

```bash
dotnet run
```

---

## Spuštění testů

Projekt podporuje **unit testy**:

```bash
dotnet test
```

Testy zatim ověřují jen  správné zpracování zpráv a ukládání do databáze.

---

## Budoucí rozšíření / To-Do
* [ ] Pokročilé filtrování a vyhledávání zpráv
* [ ] Notifikace při výskytu chyb nebo výjimek

## Licence 


Bude upřesněna 
