# PowerShell script för att testa DM API:et
# Användning: .\test-dm-api.ps1

$baseUrl = "http://localhost:5164"
$apiUrl = "$baseUrl/api/posts"

Write-Host "=== Test av DM API ===" -ForegroundColor Green
Write-Host ""

# Kontrollera om API:et körs
Write-Host "Kontrollerar om API:et körs..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/openapi/v1.json" -Method Get -TimeoutSec 2 -ErrorAction Stop
    Write-Host "✓ API:et körs!" -ForegroundColor Green
} catch {
    Write-Host "✗ API:et körs inte. Starta det först med: cd backend\SocialTDD.Api; dotnet run" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== Steg 1: Skapa testanvändare (manuellt i databasen) ===" -ForegroundColor Cyan
Write-Host "Du behöver först skapa användare i databasen." -ForegroundColor Yellow
Write-Host "Kör detta SQL i SQL Server Management Studio eller Azure Data Studio:" -ForegroundColor Yellow
Write-Host ""
Write-Host "USE SocialTDD;" -ForegroundColor White
Write-Host "GO" -ForegroundColor White
Write-Host ""
Write-Host "INSERT INTO Users (Id, Username, Email, CreatedAt)" -ForegroundColor White
Write-Host "VALUES " -ForegroundColor White
Write-Host "    (NEWID(), 'testuser1', 'user1@test.com', GETUTCDATE())," -ForegroundColor White
Write-Host "    (NEWID(), 'testuser2', 'user2@test.com', GETUTCDATE());" -ForegroundColor White
Write-Host "GO" -ForegroundColor White
Write-Host ""
Write-Host "SELECT Id, Username FROM Users;" -ForegroundColor White
Write-Host "GO" -ForegroundColor White
Write-Host ""

$userId1 = Read-Host "Ange User1 ID (eller tryck Enter för att hoppa över)"
$userId2 = Read-Host "Ange User2 ID (eller tryck Enter för att hoppa över)"

if ([string]::IsNullOrWhiteSpace($userId1) -or [string]::IsNullOrWhiteSpace($userId2)) {
    Write-Host "Hoppar över API-test. Kör SQL-kommandona ovan först." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "=== Steg 2: Skapa första meddelandet (User1 -> User2) ===" -ForegroundColor Cyan

$body1 = @{
    senderId = $userId1
    recipientId = $userId2
    message = "Hej! Detta är mitt första meddelande till dig."
} | ConvertTo-Json

try {
    $response1 = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $body1 -ContentType "application/json"
    Write-Host "✓ Meddelande skapat!" -ForegroundColor Green
    Write-Host "  ID: $($response1.id)" -ForegroundColor Gray
    Write-Host "  Meddelande: $($response1.message)" -ForegroundColor Gray
} catch {
    Write-Host "✗ Fel vid skapande av meddelande: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "  Detaljer: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== Steg 3: Skapa svar (User2 -> User1) ===" -ForegroundColor Cyan

$body2 = @{
    senderId = $userId2
    recipientId = $userId1
    message = "Tack för meddelandet! Hur är läget?"
} | ConvertTo-Json

try {
    $response2 = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $body2 -ContentType "application/json"
    Write-Host "✓ Svar skapat!" -ForegroundColor Green
    Write-Host "  ID: $($response2.id)" -ForegroundColor Gray
    Write-Host "  Meddelande: $($response2.message)" -ForegroundColor Gray
} catch {
    Write-Host "✗ Fel vid skapande av svar: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "  Detaljer: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== Steg 4: Hämta konversation mellan User1 och User2 ===" -ForegroundColor Cyan

$conversationUrl = "$apiUrl/conversation/$userId1/$userId2"

try {
    $conversation = Invoke-RestMethod -Uri $conversationUrl -Method Get
    Write-Host "✓ Konversation hämtad!" -ForegroundColor Green
    Write-Host "  Antal meddelanden: $($conversation.Count)" -ForegroundColor Gray
    Write-Host ""
    
    foreach ($msg in $conversation) {
        $sender = if ($msg.senderId -eq $userId1) { "User1" } else { "User2" }
        $recipient = if ($msg.recipientId -eq $userId1) { "User1" } else { "User2" }
        $time = [DateTime]::Parse($msg.createdAt).ToString("yyyy-MM-dd HH:mm:ss")
        Write-Host "  [$time] $sender -> $recipient : $($msg.message)" -ForegroundColor White
    }
} catch {
    Write-Host "✗ Fel vid hämtning av konversation: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "  Detaljer: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== Steg 5: Testa felhantering ===" -ForegroundColor Cyan

# Testa med ogiltigt användar-ID
Write-Host "Testar med ogiltigt användar-ID..." -ForegroundColor Yellow
$invalidUserId = "00000000-0000-0000-0000-000000000001"
try {
    $invalidUrl = "$apiUrl/conversation/$invalidUserId/$userId2"
    Invoke-RestMethod -Uri $invalidUrl -Method Get -ErrorAction Stop
    Write-Host "✗ Förväntade fel men fick svar" -ForegroundColor Red
} catch {
    Write-Host "✓ Korrekt felhantering: $($_.Exception.Message)" -ForegroundColor Green
}

# Testa med samma användare
Write-Host "Testar med samma användare..." -ForegroundColor Yellow
try {
    $sameUserUrl = "$apiUrl/conversation/$userId1/$userId1"
    Invoke-RestMethod -Uri $sameUserUrl -Method Get -ErrorAction Stop
    Write-Host "✗ Förväntade fel men fick svar" -ForegroundColor Red
} catch {
    Write-Host "✓ Korrekt felhantering: $($_.Exception.Message)" -ForegroundColor Green
}

Write-Host ""
Write-Host "=== Test klart! ===" -ForegroundColor Green


