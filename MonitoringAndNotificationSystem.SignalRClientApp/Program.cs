using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR.Client;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var hubUrl = configuration["SignalRConfig:SignalRUrl"];
        if (string.IsNullOrEmpty(hubUrl))
        {
            Console.WriteLine("Missing SignalRUrl in appsettings.json");
            return;
        }

        var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        connection.On<string>("AnomalyMessage", message =>
        {
            Console.WriteLine($"Alert received: {message}");
        });

        try
        {
            await connection.StartAsync();
            Console.WriteLine($"Connected to SignalR at {hubUrl}");
            await Task.Delay(-1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect: {ex}");
            await Task.Delay(-1);
        }
    }
}