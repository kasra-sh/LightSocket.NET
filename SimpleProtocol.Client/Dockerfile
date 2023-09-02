FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LightSocket.Client/LightSocket.Client.csproj", "LightSocket.Client/"]
RUN dotnet restore "LightSocket.Client/LightSocket.Client.csproj"
COPY . .
WORKDIR "/src/LightSocket.Client"
RUN dotnet build "LightSocket.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LightSocket.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LightSocket.Client.dll"]
