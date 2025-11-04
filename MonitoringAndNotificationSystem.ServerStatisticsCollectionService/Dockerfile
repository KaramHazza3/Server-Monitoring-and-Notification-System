FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["MonitoringAndNotificationSystem.ServerStatisticsCollectionService/MonitoringAndNotificationSystem.ServerStatisticsCollectionService.csproj", "MonitoringAndNotificationSystem.ServerStatisticsCollectionService/"]
COPY ["MonitoringAndNotificationSystem.MessageBroker/MonitoringAndNotificationSystem.MessageBroker.csproj", "MonitoringAndNotificationSystem.MessageBroker/"]

RUN dotnet restore "MonitoringAndNotificationSystem.ServerStatisticsCollectionService/MonitoringAndNotificationSystem.ServerStatisticsCollectionService.csproj"
COPY . .

WORKDIR /src/MonitoringAndNotificationSystem.ServerStatisticsCollectionService
RUN dotnet build "MonitoringAndNotificationSystem.ServerStatisticsCollectionService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringAndNotificationSystem.ServerStatisticsCollectionService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringAndNotificationSystem.ServerStatisticsCollectionService.dll"]