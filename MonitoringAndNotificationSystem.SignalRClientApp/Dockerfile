FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["MonitoringAndNotificationSystem.SignalRClientApp/MonitoringAndNotificationSystem.SignalRClientApp.csproj", "MonitoringAndNotificationSystem.SignalRClientApp/"]
RUN dotnet restore "MonitoringAndNotificationSystem.SignalRClientApp/MonitoringAndNotificationSystem.SignalRClientApp.csproj"

COPY . .

WORKDIR /src/MonitoringAndNotificationSystem.SignalRClientApp
RUN dotnet build "MonitoringAndNotificationSystem.SignalRClientApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringAndNotificationSystem.SignalRClientApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringAndNotificationSystem.SignalRClientApp.dll"]