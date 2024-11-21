FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VStore.API/VStore.API.csproj", "VStore.API/"]
COPY ["VStore.Application/VStore.Application.csproj", "VStore.Application/"]
COPY ["VStore.Domain/VStore.Domain.csproj", "VStore.Domain/"]
COPY ["VStore.Infrastructure/VStore.Infrastructure.csproj", "VStore.Infrastructure/"]
COPY ["VStore.Persistence/VStore.Persistence.csproj", "VStore.Persistence/"]
RUN dotnet restore "VStore.API/VStore.API.csproj"
COPY . .
WORKDIR "/src/VStore.API"
RUN dotnet build "VStore.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "VStore.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VStore.API.dll"]