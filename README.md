# Portfolio projektu
https://github.com/Grimhandy321/Portfolio/tree/main


# Monologer

**Monologer** je C# aplikace na **.NET 9**, která přijímá WebSocket zprávy a ukládá je do **MS SQL Server**. Projekt slouží jako ukázka práce s WebSockety a na  paralelizací.

---

## Předpoklady

net ef
.net 9
MSSQL

## (jen na ucitelských pc )
instalace .net ef tools 
```bash
cd Monologger
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
cd Monologger
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

---

## Dokumentace 

# WebSocket API – Monologer

Tento dokument popisuje WebSocket endpoint backendu **Monologer** pro zpracování zpráv v reálném čase.

---

## Základní URL

```
/ws
```

---

## Připojení k WebSocket

### Endpoint

```
GET /ws/connect
```

### Popis

Tento endpoint zahajuje WebSocket handshake a upgradne HTTP spojení na persistentní WebSocket spojení. Po připojení může klient posílat JSON zprávy, které budou validovány, obohaceny o informace o uživateli a zařazeny do fronty pro zpracování.

Endpoint je viditelný ve Swaggeru, ale je určen **pouze pro WebSocket upgrade**, nikoli pro standardní HTTP požadavky.

---

## Autentizace

Endpoint `/ws/connect` je chráněn **middlewarem pro tokenovou autentizaci**.

### Hlavička

Klient musí posílat hlavičku `Authorization` s platným přístupovým tokenem (access_token):

```
Authorization: YOUR_ACCESS_TOKEN
```

* Požadavky **bez hlavičky** nebo s neplatným tokenem obdrží `401 Unauthorized`
* Pouze WebSocket endpointy (`/ws/*`) jsou chráněny; jiné cesty, např. `/swagger`, nejsou kontrolovány
* Access token se získá po registraci uživatele přes endpoint:

```csharp
[HttpPost]
[Route("api/users/create")]
public async Task<IActionResult> CreateUser([FromForm] UserDto userDto)
```

Příklad odpovědi po vytvoření uživatele:

```json
{
  "accessKey": "generated_random_token"
}
```

---

## Životní cyklus WebSocket

1. Klient posílá `GET /ws/connect` s WebSocket hlavičkami.
2. Server upgradne spojení na WebSocket.
3. Klient posílá textové JSON zprávy.
4. Server validuje a zpracuje každou zprávu.
5. Server odpoví krátkou textovou zprávou.

---

## Formát zprávy

Zprávy musí být posílány jako UTF-8 JSON text.

### JSON Schema

```json
{
  "text": "string",
  "magnitude": 0,
  "type": "Error | Info | Warning"
}
```
## TypeScript typy

```ts
type MessageType = 'Error' | 'Info' | 'Warning';

interface Message {
  text: string;
  magnitude: number;
  type: MessageType;
}

type ServerResponse = 'accomplished' | 'invalid_message_format' | 'userNotfound';
```

---

## Serverové odpovědi

| Text odpovědi            | Popis                                                |
| ------------------------ | ---------------------------------------------------- |
| `accomplished`           | Zpráva byla přijata, validována a zařazena do fronty |
| `invalid_message_format` | Zpráva nebyla ve spravnem formatu                    |
| `userNotfound`           | Kontext uživatele nebyl nalezen                      |

---

## Příklad TypeScript klienta

```ts
const socket = new WebSocket("wss://your-domain/ws/connect", [], {
  headers: {
    Authorization: "YOUR_ACCESS_TOKEN" // získaný z api/users/create
  }
});

interface Message {
  text: string;
  magnitude: number;
  type: 'Error' | 'Info' | 'Warning';
}

type ServerResponse = 'accomplished' | 'invalid_message_format' | 'userNotfound';

socket.onopen = () => {
  const message: Message = {
    text: "Systém inicializován",
    magnitude: 1,
    type: 1
  };
  socket.send(JSON.stringify(message));
};

socket.onmessage = (event) => {
  const response: ServerResponse = event.data as ServerResponse;
  console.log("Odpověď serveru:", response);
};
```

---

## Ukončení spojení

Pokud klient pošle WebSocket Close frame:

* Server odpoví `NormalClosure`
* Spojení je ukončeno bezpečně

---

## Autor

**Michal Příhoda**

---

## Poznámky
* Zprávy musí být posílány jako **text frames**
* Binary frames nejsou podporovány
