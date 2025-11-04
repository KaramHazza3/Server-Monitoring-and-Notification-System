using System.Diagnostics;
using ServerStatisticsCollectionService.Data;
using ServerStatisticsCollectionService.Services.ServerStatisticsCollector.Intf;

namespace ServerStatisticsCollectionService.Services.ServerStatisticsCollector;

public class ServerStatisticsCollector : IServerStatisticsCollector
{
    private readonly Process _currentProcess;
    private TimeSpan _prevTotalProcessorTime;
    private DateTime _prevCheckTime;

    public ServerStatisticsCollector()
    {
        _currentProcess = Process.GetCurrentProcess();
        _prevTotalProcessorTime = _currentProcess.TotalProcessorTime;
        _prevCheckTime = DateTime.UtcNow;
    }

    public async Task<ServerStatistics> CollectStatisticsAsync(int intervalSeconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(intervalSeconds));

        double cpuUsage = GetCpuUsage();

        double availableMemory = GetAvailableMemoryMb();
        double processMemoryMb = _currentProcess.WorkingSet64 / (1024.0 * 1024.0);

        return new ServerStatistics
        {
            CpuUsage = cpuUsage,
            AvailableMemory = availableMemory,
            MemoryUsage = processMemoryMb,
            Timestamp = DateTime.Now
        };
    }

    private double GetCpuUsage()
    {
        var currentTotalProcessorTime = _currentProcess.TotalProcessorTime;
        var currentTime = DateTime.UtcNow;

        var cpuUsedMs = (currentTotalProcessorTime - _prevTotalProcessorTime).TotalMilliseconds;
        var totalMsPassed = (currentTime - _prevCheckTime).TotalMilliseconds;
        int cpuCount = Environment.ProcessorCount;

        _prevTotalProcessorTime = currentTotalProcessorTime;
        _prevCheckTime = currentTime;

        if (totalMsPassed <= 0) return 0;

        double cpuUsage = (cpuUsedMs / (totalMsPassed * cpuCount)) * 100.0;
        return Math.Clamp(cpuUsage, 0, 100);
    }

    private double GetAvailableMemoryMb()
    {
        if (OperatingSystem.IsWindows())
        {
            var gcInfo = GC.GetGCMemoryInfo();
            return gcInfo.TotalAvailableMemoryBytes / (1024.0 * 1024.0);
        }
        if (OperatingSystem.IsLinux() && File.Exists("/proc/meminfo"))
        {
            var memInfo = File.ReadAllLines("/proc/meminfo");
            var memAvailableLine = memInfo.FirstOrDefault(line => line.StartsWith("MemAvailable:"));
            if (memAvailableLine != null)
            {
                var parts = memAvailableLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (double.TryParse(parts[1], out double kb))
                    return kb / 1024.0;
            }
        }

        return GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024.0 * 1024.0);
    }
}
