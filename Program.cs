using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using WinISA.Collectors;
using WinISA.Helpers;
using WinISA.Models;

namespace WinISA;

class Program
{
    static void Main()
    {
        try
        {
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };
            SetConsoleCtrlHandler(type =>
            {
                if (type == CtrlType.CTRL_SHUTDOWN_EVENT || type == CtrlType.CTRL_LOGOFF_EVENT)
                    cts.Cancel();
                return false;
            }, true);

            var info = CollectSystemInfo();
            SaveInfoToFile(info);

            using var waitHandle = new ManualResetEvent(false);
            using var registration = cts.Token.Register(() => waitHandle.Set());
            waitHandle.WaitOne();
        }
        catch (Exception ex)
        {
            File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "winisa_fatal_error.txt"), $"Fatal error at {DateTime.Now}:{Environment.NewLine}{ex}");
            throw;
        }
    }

    private static SystemInfo CollectSystemInfo()
    {
        var info = new SystemInfo
        {
            Timestamp = DateTime.Now,
            WindowsVersion = SafeGet(WindowsCollector.GetVersion),
            TimeZone = TimeZoneInfo.Local.DisplayName,
            TimeZoneId = TimeZoneInfo.Local.Id,
            UtcOffset = TimeZoneInfo.Local.BaseUtcOffset.ToString(),
            CPU = SafeGet(CpuCollector.GetInfo),
            GPU = SafeGet(GpuCollector.GetInfo),
            RAM = SafeGet(RamCollector.GetInfo),
            Disks = SafeGet(DiskCollector.GetInfo, new List<DiskInfo>()),
            NetworkAdapters = SafeGet(NetworkCollector.GetInfo, new List<NetworkAdapterInfo>()),
            WiFi = SafeGet(WiFiCollector.GetInfo),
            Drivers = SafeGet(DriverCollector.GetInfo, new List<DriverInfo>()),
            Services = SafeGet(ServiceCollector.GetInfo, new List<ServiceInfo>()),
            ProductKey = SafeGet(WindowsCollector.GetProductKey, "Unavailable"),
            Username = Environment.UserDomainName + "\\" + Environment.UserName,
            MicrosoftAccountEmail = SafeGet(WindowsCollector.GetMicrosoftAccountEmail, "Unavailable"),
            Motherboard = SafeGet(MotherboardCollector.GetInfo),
            BIOS = SafeGet(BiosCollector.GetInfo),
            SystemUptime = SafeGet(() => TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"dd\d\ hh\h\ mm\m\ ss\s"), "Unknown"),
            InstalledSoftware = SafeGet(SoftwareCollector.GetInfo, new List<SoftwareInfo>()),
            RunningProcesses = SafeGet(ProcessCollector.GetInfo, new List<ProcessInfo>()),
            StartupItems = SafeGet(StartupCollector.GetInfo, new List<StartupItemInfo>()),
            InstalledHotfixes = SafeGet(HotfixCollector.GetInfo, new List<HotfixInfo>()),
            Printers = SafeGet(PrinterCollector.GetInfo, new List<PrinterInfo>()),
            AudioDevices = SafeGet(AudioCollector.GetInfo, new List<AudioDeviceInfo>()),
            HostsFileEntries = SafeGet(HostsCollector.GetEntries, new List<string>()),
            MappedDrives = SafeGet(MappedDriveCollector.GetInfo, new List<MappedDriveInfo>()),
            EnvironmentVariables = SafeGet(EnvironmentCollector.GetInfo, new List<EnvironmentVariableInfo>()),
            Battery = SafeGet(BatteryCollector.GetInfo),
            SystemBootTime = SafeGet(() => WindowsCollector.GetBootTime(), "Unknown"),
            LocalUsers = SafeGet(UserCollector.GetLocalUsers, new List<LocalUserInfo>()),
            InstalledFonts = SafeGet(FontCollector.GetFonts, new List<string>()),
            ActivationStatus = SafeGet(ActivationCollector.GetStatus),
            ListeningPorts = SafeGet(PortCollector.GetListeningPorts, new List<PortInfo>()),
            EventLogErrors = SafeGet(EventLogCollector.GetRecentErrors, new List<EventLogEntryInfo>()),
            WindowsFeatures = SafeGet(FeatureCollector.GetFeatures, new List<WindowsFeatureInfo>()),
            ScheduledTasks = SafeGet(TaskCollector.GetTasks, new List<ScheduledTaskInfo>()),
            NetworkShares = SafeGet(ShareCollector.GetShares, new List<NetworkShareInfo>()),
            ProcessDetails = SafeGet(ProcessDetailCollector.GetInfo, new List<ProcessDetailInfo>()),
            Certificates = SafeGet(CertificateCollector.GetCertificates, new List<CertificateInfo>()),
            WindowsUpdateHistory = SafeGet(UpdateCollector.GetHistory, new List<UpdateHistoryInfo>()),
            FirewallRules = SafeGet(FirewallCollector.GetRules, new List<FirewallRuleInfo>()),
            UwpApps = SafeGet(UwpCollector.GetApps, new List<UwpAppInfo>()),
            BluetoothDevices = SafeGet(BluetoothCollector.GetDevices, new List<BluetoothDeviceInfo>()),
            UsbDevices = SafeGet(UsbCollector.GetDevices, new List<UsbDeviceInfo>()),
            DiskPartitions = SafeGet(PartitionCollector.GetPartitions, new List<DiskPartitionInfo>()),
            SystemLocale = SafeGet(LocaleCollector.GetInfo),
            DotNetFrameworks = SafeGet(DotNetCollector.GetFrameworks, new List<DotNetFrameworkInfo>()),
            PowerSchemes = SafeGet(PowerCollector.GetSchemes, new List<PowerSchemeInfo>()),
            DirectXVersion = SafeGet(DirectXCollector.GetVersion, "Unknown")
        };

        // Compute hardware ID using CPU, Motherboard, System Drive, and MAC
        info.HardwareId = ComputeHardwareId(info);

        return info;
    }

    private static T SafeGet<T>(Func<T> func, T defaultValue = default!)
    {
        try { return func(); }
        catch { return defaultValue; }
    }

    private static void SaveInfoToFile(SystemInfo info)
    {
        try
        {
            var file = Path.Combine(AppContext.BaseDirectory, "system_info.json");
            var json = JsonSerializer.Serialize(info, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            File.WriteAllText(file, json, Encoding.UTF8);
            File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "winisa_ran.txt"), DateTime.Now.ToString());
        }
        catch (Exception ex)
        {
            File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "winisa_save_error.txt"), $"Save error at {DateTime.Now}:{Environment.NewLine}{ex}");
        }
    }

    private static string ComputeHardwareId(SystemInfo info)
    {
        var parts = new List<string>();

        // 1. CPU ProcessorId (most reliable)
        if (!string.IsNullOrEmpty(info.CPU?.ProcessorId))
            parts.Add(info.CPU.ProcessorId);
        else if (info.CPU != null)
            // fallback: combine other CPU attributes
            parts.Add($"{info.CPU.Manufacturer}{info.CPU.Name}{info.CPU.MaxClockSpeed}{info.CPU.Cores}{info.CPU.LogicalProcessors}");

        // 2. Motherboard Serial
        if (!string.IsNullOrEmpty(info.Motherboard?.SerialNumber))
            parts.Add(info.Motherboard.SerialNumber);
        else if (info.Motherboard != null)
            parts.Add($"{info.Motherboard.Manufacturer}{info.Motherboard.Model}{info.Motherboard.Version}");

        // 3. System Drive Serial (the disk where Windows is installed)
        var systemDrive = info.Disks?.FirstOrDefault(d => d.DeviceID == Environment.GetEnvironmentVariable("SystemDrive") + "\\");
        if (systemDrive != null && !string.IsNullOrEmpty(systemDrive.SerialNumber))
            parts.Add(systemDrive.SerialNumber);
        else if (info.Disks?.Count > 0)
        {
            var firstWithSerial = info.Disks.FirstOrDefault(d => !string.IsNullOrEmpty(d.SerialNumber));
            if (firstWithSerial != null)
                parts.Add(firstWithSerial.SerialNumber);
            else
                parts.Add(info.Disks[0].Model ?? "UnknownDisk");
        }

        // 4. Primary MAC (first active adapter)
        var primaryMac = info.NetworkAdapters?.FirstOrDefault()?.MAC;
        if (!string.IsNullOrEmpty(primaryMac))
            parts.Add(primaryMac);

        if (parts.Count == 0)
            return "Unknown";

        var combined = string.Join("|", parts);
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
        return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
    }

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleCtrlHandler(CtrlHandlerRoutine handler, bool add);
    private delegate bool CtrlHandlerRoutine(CtrlType type);
    private enum CtrlType
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT = 1,
        CTRL_CLOSE_EVENT = 2,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT = 6
    }
}