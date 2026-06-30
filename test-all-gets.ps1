# ============================================================================
# Script: test-all-gets.ps1
# Descripción: Ejecuta todos los GET endpoints de la API TesisTIC
# URL Base: http://localhost:5000/api
# ============================================================================

param(
    [string]$BaseUrl = "http://localhost:5000",
    [string]$OutputFile = "resultados-get.log"
)

# ============================================================================
# CONFIGURACIÓN
# ============================================================================

$apiUrl = "$BaseUrl/api"
$healthUrl = "$BaseUrl/health"
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "         TEST COMPLETO - TODOS LOS GET               " -ForegroundColor Cyan
Write-Host "            TesisTIC API v1.0                        " -ForegroundColor Cyan
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Inicio: $timestamp"
Write-Host "API URL: $apiUrl"
Write-Host "Archivo de log: $OutputFile"
Write-Host ""

# Inicializar archivo de log
"=======================================================" | Out-File -FilePath $OutputFile -Force
"TEST COMPLETO - TODOS LOS GET ENDPOINTS" | Out-File -FilePath $OutputFile -Append
"=======================================================" | Out-File -FilePath $OutputFile -Append
"Timestamp: $timestamp" | Out-File -FilePath $OutputFile -Append
"Base URL: $BaseUrl" | Out-File -FilePath $OutputFile -Append
"" | Out-File -FilePath $OutputFile -Append

# ============================================================================
# FUNCIÓN PARA HACER REQUESTS
# ============================================================================

function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Method = "GET",
        [string]$Url,
        [int]$Expected = 200
    )
    
    Write-Host "--- $Name" -ForegroundColor Yellow
    Write-Host "    Metodo: $Method | URL: $Url" -ForegroundColor Gray
    
    try {
        $response = Invoke-WebRequest -Uri $Url -Method $Method -ErrorAction SilentlyContinue -UseBasicParsing
        if ($null -ne $response) {
            $statusCode = $response.StatusCode
        }
        else {
            $statusCode = 0
        }
        
        if ($statusCode -eq $Expected) {
            Write-Host "    OK Status: $statusCode" -ForegroundColor Green
            
            # Parsear y mostrar resultado
            try {
                $jsonContent = $response.Content | ConvertFrom-Json
                $contentPreview = $response.Content
                if ($contentPreview.Length -gt 200) {
                    $contentPreview = $contentPreview.Substring(0, 200) + "..."
                }
                Write-Host "    Respuesta: $contentPreview" -ForegroundColor Cyan
            }
            catch {
                Write-Host "    Respuesta: $($response.Content)" -ForegroundColor Cyan
            }
            
            # Log
            "OK $Name" | Out-File -FilePath $OutputFile -Append
            "   Status: $statusCode" | Out-File -FilePath $OutputFile -Append
            "   URL: $Url" | Out-File -FilePath $OutputFile -Append
        }
        else {
            Write-Host "    WARNING Status: $statusCode (esperado: $Expected)" -ForegroundColor Yellow
            "WARNING $Name - Status: $statusCode (esperado: $Expected)" | Out-File -FilePath $OutputFile -Append
        }
    }
    catch {
        Write-Host "    ERROR: $($_.Exception.Message)" -ForegroundColor Red
        "ERROR $Name - Error: $($_.Exception.Message)" | Out-File -FilePath $OutputFile -Append
    }
    
    Write-Host ""
}

# ============================================================================
# TEST 1: HEALTH CHECK
# ============================================================================

Write-Host "[1] HEALTH CHECK" -ForegroundColor Magenta
Write-Host "-------------------------------------------" -ForegroundColor Gray
Test-Endpoint -Name "Health Check" -Url "$healthUrl"

# ============================================================================
# TEST 2: PROPUESTAS
# ============================================================================

Write-Host "[2] PROPUESTAS" -ForegroundColor Magenta
Write-Host "-------------------------------------------" -ForegroundColor Gray
Test-Endpoint -Name "GET: Todas las propuestas" -Url "$apiUrl/propuestas"
Test-Endpoint -Name "GET: Propuestas por estado BORRADOR" -Url "$apiUrl/propuestas?estado=BORRADOR"
Test-Endpoint -Name "GET: Propuestas por estado PENDIENTE" -Url "$apiUrl/propuestas?estado=PENDIENTE"
Test-Endpoint -Name "GET: Propuestas por estado APROBADA" -Url "$apiUrl/propuestas?estado=APROBADA"
Test-Endpoint -Name "GET: Propuesta por ID 1" -Url "$apiUrl/propuestas/1"
Test-Endpoint -Name "GET: Propuesta por ID 999 no existe" -Url "$apiUrl/propuestas/999" -Expected 404

# ============================================================================
# TEST 3: DOCENTES
# ============================================================================

Write-Host "[3] DOCENTES" -ForegroundColor Magenta
Write-Host "-------------------------------------------" -ForegroundColor Gray
Test-Endpoint -Name "GET: Todos los docentes" -Url "$apiUrl/docentes"
Test-Endpoint -Name "GET: Docente por ID 1" -Url "$apiUrl/docentes/1"
Test-Endpoint -Name "GET: Docente por ID 999 no existe" -Url "$apiUrl/docentes/999" -Expected 404

# ============================================================================
# TEST 4: ASIGNATURAS
# ============================================================================

Write-Host "[4] ASIGNATURAS" -ForegroundColor Magenta
Write-Host "-------------------------------------------" -ForegroundColor Gray
Test-Endpoint -Name "GET: Todas las asignaturas" -Url "$apiUrl/asignaturas"
Test-Endpoint -Name "GET: Asignatura por ID 1" -Url "$apiUrl/asignaturas/1"
Test-Endpoint -Name "GET: Asignatura por ID 999 no existe" -Url "$apiUrl/asignaturas/999" -Expected 404

# ============================================================================
# TEST 5: ESTUDIANTES
# ============================================================================

Write-Host "[5] ESTUDIANTES" -ForegroundColor Magenta
Write-Host "-------------------------------------------" -ForegroundColor Gray
Test-Endpoint -Name "GET: Todos los estudiantes" -Url "$apiUrl/estudiantes"
Test-Endpoint -Name "GET: Estudiante por ID 1" -Url "$apiUrl/estudiantes/1"
Test-Endpoint -Name "GET: Estudiante por ID 999 no existe" -Url "$apiUrl/estudiantes/999" -Expected 404

# ============================================================================
# RESUMEN
# ============================================================================

Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "                 PRUEBAS COMPLETADAS                " -ForegroundColor Cyan
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "OK - Archivo de log guardado en: $OutputFile"
Write-Host "INFO - Para ver resultados detallados ejecuta:"
Write-Host "   Get-Content $OutputFile" -ForegroundColor Green
Write-Host ""
