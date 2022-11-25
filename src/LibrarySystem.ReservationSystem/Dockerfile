FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

EXPOSE "8070"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LibrarySystem.ReservationSystem.csproj", "LibrarySystem.ReservationSystem/"]
RUN dotnet restore "LibrarySystem.ReservationSystem/LibrarySystem.ReservationSystem.csproj"

WORKDIR "/src/LibrarySystem.ReservationSystem"
COPY . .

RUN dotnet build "LibrarySystem.ReservationSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LibrarySystem.ReservationSystem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibrarySystem.ReservationSystem.dll"]
