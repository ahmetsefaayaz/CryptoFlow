# 1. Çalışma zamanı (Runtime) imajı - Sadece projeyi çalıştırmak için hafif imaj
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. SDK imajı - Kodu derlemek (Build) için ağır imaj
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ÖNCE SADECE .csproj DOSYALARINI KOPYALA (Docker Cache'i verimli kullanmak için)
COPY ["CryptoFlow.API/CryptoFlow.API.csproj", "CryptoFlow.API/"]
COPY ["CryptoFlow.Application/CryptoFlow.Application.csproj", "CryptoFlow.Application/"]
COPY ["CryptoFlow.Domain/CryptoFlow.Domain.csproj", "CryptoFlow.Domain/"]
COPY ["CryptoFlow.Infrastructure/CryptoFlow.Infrastructure.csproj", "CryptoFlow.Infrastructure/"]
COPY ["CryptoFlow.Persistence/CryptoFlow.Persistence.csproj", "CryptoFlow.Persistence/"]

# Bağımlılıkları (Nuget paketlerini) indir
RUN dotnet restore "CryptoFlow.API/CryptoFlow.API.csproj"

# ŞİMDİ TÜM KODLARI KOPYALA VE DERLE
COPY . .
WORKDIR "/src/CryptoFlow.API"
RUN dotnet build "CryptoFlow.API.csproj" -c Release -o /app/build

# 3. Publish işlemi (Gereksiz dosyaları at, sadece çalışacak olanları al)
FROM build AS publish
RUN dotnet publish "CryptoFlow.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Son aşama: Publish edilmiş dosyaları hafif Runtime imajına al ve ayağa kaldır
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoFlow.API.dll"]