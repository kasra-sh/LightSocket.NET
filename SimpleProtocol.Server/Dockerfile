FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LightSocket.Server/LightSocket.Server.csproj", "LightSocket.Server/"]
RUN dotnet restore "LightSocket.Server/LightSocket.Server.csproj"
COPY . .
WORKDIR "/src/LightSocket.Server"
RUN dotnet build "LightSocket.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LightSocket.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LightSocket.Server.dll"]
