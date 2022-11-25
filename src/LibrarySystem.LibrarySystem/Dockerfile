#FROM bitnami/aspnet-core:6 AS base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

EXPOSE "8060"

FROM  mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LibrarySystem.LibrarySystem.csproj", "LibrarySystem.LibrarySystem/"]
RUN dotnet restore "LibrarySystem.LibrarySystem/LibrarySystem.LibrarySystem.csproj"

WORKDIR "/src/LibrarySystem.LibrarySystem"
COPY . .

RUN dotnet build "LibrarySystem.LibrarySystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LibrarySystem.LibrarySystem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibrarySystem.LibrarySystem.dll"]
