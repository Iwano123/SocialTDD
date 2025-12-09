# Direct Messages - TDD Implementation

## ✅ Genomförda TDD-cykler

### 1. RED → GREEN: Grundläggande funktionalitet
- **Test**: `SendDirectMessage_WhenRecipientExists_ShouldSucceed`
- **Implementation**: Minimal `DirectMessageService` som skapar `DirectMessage`

### 2. RED → GREEN: Validering av tomma meddelanden
- **Test**: `SendDirectMessage_WhenContentIsEmpty_ShouldThrowException`
- **Implementation**: Validering av `string.IsNullOrWhiteSpace(content)`

### 3. RED → GREEN: Validering av mottagare
- **Test**: `SendDirectMessage_WhenRecipientDoesNotExist_ShouldThrowException`
- **Implementation**: Repository pattern med `IUserRepository` för att verifiera att mottagare existerar

### 4. RED → GREEN: Hämta mottagna meddelanden
- **Test**: `GetReceivedMessages_ShouldReturnOnlyMessagesForUser`
- **Implementation**: `IDirectMessageRepository` med metod för att hämta meddelanden sorterade i kronologisk ordning

### 5. RED → GREEN: Maxlängd-validering
- **Test**: `SendDirectMessage_WhenContentExceedsMaxLength_ShouldThrowException`
- **Implementation**: Validering av max 1000 tecken

### 6. RED → GREEN: Förhindra att skicka till sig själv
- **Test**: `SendDirectMessage_WhenSendingToSelf_ShouldThrowException`
- **Implementation**: Validering som förhindrar att senderId == recipientId

## 📁 Projektstruktur

```
Backend/
├── SocialTDD.Domain/          # Domänmodeller
│   └── Models/
│       ├── DirectMessage.cs
│       └── User.cs
├── SocialTDD.Data/            # Repository interfaces & implementations
│   ├── Contexts/
│   │   └── SocialDbContext.cs
│   └── Repositories/
│       ├── IDirectMessageRepository.cs
│       ├── DirectMessageRepository.cs
│       ├── IUserRepository.cs
│       └── UserRepository.cs
├── SocialTDD.Services/        # Business logic
│   ├── Interfaces/
│   │   └── IDirectMessageService.cs
│   └── DirectMessageService.cs
├── SocialTDD.Api/            # Web API
│   ├── Controllers/
│   │   └── DirectMessagesController.cs
│   ├── Models/
│   │   ├── Requests/
│   │   │   └── SendDirectMessageRequest.cs
│   │   └── Responses/
│   │       ├── DirectMessageResponse.cs
│   │       └── ErrorResponse.cs
│   ├── Program.cs
│   └── appsettings.json
└── SocialTDD.Tests/           # Enhetstester
    └── UnitTests/
        └── Services/
            └── DirectMessageServiceTests.cs
```

## 🧪 Testfall (6 st)

1. ✅ Skicka DM när mottagare existerar
2. ✅ Validera tomma meddelanden
3. ✅ Validera att mottagare existerar
4. ✅ Hämta mottagna meddelanden (sorterade)
5. ✅ Validera maxlängd (1000 tecken)
6. ✅ Förhindra att skicka till sig själv

## 🚀 API Endpoints

### POST /api/directmessages
Skicka ett direktmeddelande
```json
{
  "senderId": 1,
  "recipientId": 2,
  "content": "Hello!"
}
```

### GET /api/directmessages/received/{userId}
Hämta alla mottagna meddelanden för en användare

### GET /api/directmessages/conversation/{userId1}/{userId2}
Hämta konversation mellan två användare

## 🗄️ Databas

Använder Entity Framework Core med SQL Server. Connection string finns i `appsettings.json`.

För att skapa databasen:
```bash
cd Backend/SocialTDD.Api
dotnet ef migrations add InitialCreate --project ../SocialTDD.Data
dotnet ef database update --project ../SocialTDD.Data
```

## ✅ Alla krav uppfyllda

- ✅ Enhetstester för DM-logik
- ✅ Validering av input (tomma meddelanden, maxlängd)
- ✅ Edge cases (cirkulära relationer, ogiltiga användare, skicka till sig själv)
- ✅ Repository pattern
- ✅ API Controller med felhantering
- ✅ EF Core implementation
- ✅ Clean Code-principer (tydliga namn, små funktioner, separerad logik)

