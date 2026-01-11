# SocialTDD

Socialt nätverk byggt med Test-Driven Development (TDD) och Clean Code-principer.

## 📋 Projektöversikt

Detta projekt är en kompletteringsuppgift för kursen och implementerar en social nätverksapplikation med fokus på:
- Test-Driven Development (TDD)
- Clean Code-principer (Robert C. Martin)
- Verifiering & testmetoder
- Versionshantering & projektarbete

## 🏗️ Teknisk Stack

- **Front-end**: React
- **Back-end**: .NET 9.0 Web API
- **Databas**: SQL Server (Entity Framework Core)
- **Versionshantering**: Git + Git Flow
- **CI/CD**: GitHub Actions

## 📦 Projektstruktur

```
SocialTDD/
├── backend/
│   ├── SocialTDD.Api/          # Web API controllers
│   ├── SocialTDD.Application/  # Business logic, services, DTOs
│   ├── SocialTDD.Domain/       # Domain entities
│   ├── SocialTDD.Infrastructure/ # Data access, repositories
│   └── SocialTDD.Tests/        # Unit tests
└── frontend/                    # React application
```

## 🚀 Setup

### Snabbstart (Rekommenderat)

Starta både backend och frontend med ett enda kommando:

**PowerShell:**
```powershell
.\start.ps1
```

**Command Prompt:**
```cmd
start.bat
```

Detta startar båda servrarna i separata fönster:
- Backend: http://localhost:5000
- Frontend: http://localhost:3000

### Manuell start

#### Backend

1. Restore dependencies:
   ```bash
   dotnet restore
   ```

2. Konfigurera databas i `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SocialTDD;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

3. Kör migrations:
   ```bash
   dotnet ef database update --project backend/SocialTDD.Infrastructure --startup-project backend/SocialTDD.Api
   ```

4. Kör API:
   ```bash
   dotnet run --project backend/SocialTDD.Api
   ```

### Frontend

1. Installera dependencies:
   ```bash
   cd frontend
   npm install
   ```

2. Starta utvecklingsserver:
   ```bash
   npm start
   ```

## 🧪 Testning

### Backend-tester

```bash
dotnet test
```

**Testresultat:**
- ✅ 72 tester passerar
- ❌ 0 tester misslyckades
- ⏱️ Total tid: ~1 sekund

### Coverage-rapport

Generera coverage-rapport lokalt:

```bash
# Kör tester med coverage
dotnet test --configuration Release --collect:"XPlat Code Coverage" --results-directory:"./TestResults" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura

# Generera HTML-rapport
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./CoverageReport" -reporttypes:"Html;Badges;TextSummary"
```

Öppna `./CoverageReport/index.html` för detaljerad coverage-rapport.

Se [TEST_COVERAGE.md](TEST_COVERAGE.md) för detaljerad dokumentation.

### Frontend-tester

```bash
cd frontend
npm test
```

## 📊 Statisk Kodanalys

Projektet använder .NET analyzers för statisk kodanalys. Se [STATIC_CODE_ANALYSIS.md](STATIC_CODE_ANALYSIS.md) för detaljerad dokumentation.

**Status**: ✅ Inga varningar eller fel

## 📚 Dokumentation

- [Statisk Kodanalys](STATIC_CODE_ANALYSIS.md) - Dokumentation av kodanalys och resultat
- [Test Coverage](TEST_COVERAGE.md) - Dokumentation av test coverage och resultat

## 🔐 Autentisering

API:et använder JWT-autentisering. Endpoints är skyddade med `[Authorize]` attribut.

## 📝 Funktionalitet

1. ✅ Posta inlägg på användares tidslinjer
2. ✅ Läsa tidslinje (kronologisk ordning)
3. ✅ Följa användare
4. ✅ Vägg (aggregat-flöde från följda användare)
5. ✅ Direktmeddelanden (DM)
6. ✅ Persistens (SQL Server)

## 📊 Test Coverage

- **72 tester** passerar
- **Application Layer**: 95.6% coverage ✅
- **Domain Layer**: 84.6% coverage ✅
- **Branch coverage**: 74.4%
- **Method coverage**: 59.8%
- Coverage-rapporter genereras automatiskt i CI/CD
- Se [TEST_COVERAGE.md](TEST_COVERAGE.md) för detaljerad dokumentation

## 📄 Licens

Detta projekt är en kursuppgift.