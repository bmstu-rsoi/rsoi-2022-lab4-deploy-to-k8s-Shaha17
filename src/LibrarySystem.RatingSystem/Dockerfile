FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

EXPOSE "8050"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LibrarySystem.RatingSystem.csproj", "LibrarySystem.RatingSystem/"]
RUN dotnet restore "LibrarySystem.RatingSystem/LibrarySystem.RatingSystem.csproj"

WORKDIR "/src/LibrarySystem.RatingSystem"
COPY . .

RUN dotnet build "LibrarySystem.RatingSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LibrarySystem.RatingSystem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibrarySystem.RatingSystem.dll"]
