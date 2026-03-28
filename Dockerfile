FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["AgriConnect/AgriConnect.csproj", "AgriConnect/"]
RUN dotnet restore "AgriConnect/AgriConnect.csproj"
COPY . .
WORKDIR "/src/AgriConnect"
RUN dotnet publish "AgriConnect.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AgriConnect.dll"]