# Multi-stage build para optimización
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

# Copiar archivos de proyecto
COPY ["TesisTIC.API/TesisTIC.API.csproj", "TesisTIC.API/"]
COPY ["TesisTIC.Application/TesisTIC.Application.csproj", "TesisTIC.Application/"]
COPY ["TesisTIC.Domain/TesisTIC.Domain.csproj", "TesisTIC.Domain/"]
COPY ["TesisTIC.Infrastructure/TesisTIC.Infrastructure.csproj", "TesisTIC.Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "TesisTIC.API/TesisTIC.API.csproj"

# Copiar código fuente
COPY ["TesisTIC.API/", "TesisTIC.API/"]
COPY ["TesisTIC.Application/", "TesisTIC.Application/"]
COPY ["TesisTIC.Domain/", "TesisTIC.Domain/"]
COPY ["TesisTIC.Infrastructure/", "TesisTIC.Infrastructure/"]

# Build
RUN dotnet build "TesisTIC.API/TesisTIC.API.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "TesisTIC.API/TesisTIC.API.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copiar archivos publicados desde build stage
COPY --from=build /app/publish .

# Exponer puertos
EXPOSE 5000 5001

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Punto de entrada
ENTRYPOINT ["dotnet", "TesisTIC.API.dll"]
