FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["MonitoringAndNotificationSystem.MessageProcessingAndAnomalyDetectionService/MonitoringAndNotificationSystem.MessageProcessingAndAnomalyDetectionService.csproj", "MonitoringAndNotificationSystem.MessageProcessingAndAnomalyDetectionService/"]
COPY ["MonitoringAndNotificationSystem.MessageBroker/MonitoringAndNotificationSystem.MessageBroker.csproj", "MonitoringAndNotificationSystem.MessageBroker/"]

RUN dotnet restore "MonitoringAndNotificationSystem.MessageProcessingAndAnomalyDetectionService/MonitoringAndNotificationSystem.MessageProcessingAndAnomalyDetectionService.csproj"
COPY . .

WORKDIR /src/MonitoringAndNotificationSystem.MessageProcessingAndAnomalyDetectionService
RUN dotnet build "MonitoringAndNotificationSystem.MessageProcessingAndAnomalyDetectionService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringAndNotificationSystem.MessageProcessingAndAnomalyDetectionService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitoringAndNotificationSystem.MessageProcessingAndAnomalyDetectionService.dll"]


