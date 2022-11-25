FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

EXPOSE "8080"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LibrarySystem.Gateway.csproj", "LibrarySystem.Gateway/"]
RUN dotnet restore "LibrarySystem.Gateway/LibrarySystem.Gateway.csproj"

WORKDIR "/src/LibrarySystem.Gateway"
COPY . .

RUN dotnet build "LibrarySystem.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LibrarySystem.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibrarySystem.Gateway.dll"]
