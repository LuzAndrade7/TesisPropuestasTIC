FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TesisTIC.API/TesisTIC.API.csproj", "TesisTIC.API/"]
COPY ["TesisTIC.Application/TesisTIC.Application.csproj", "TesisTIC.Application/"]
COPY ["TesisTIC.Domain/TesisTIC.Domain.csproj", "TesisTIC.Domain/"]
COPY ["TesisTIC.Infrastructure/TesisTIC.Infrastructure.csproj", "TesisTIC.Infrastructure/"]

RUN dotnet restore "TesisTIC.API/TesisTIC.API.csproj"

COPY . .
RUN dotnet build "TesisTIC.API/TesisTIC.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TesisTIC.API/TesisTIC.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "TesisTIC.API.dll"]
