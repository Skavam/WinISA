using System;
using System.Collections.Generic;

namespace WinISA.Models;

public class SystemInfo
{
    public DateTime Timestamp { get; set; }
    public WindowsVersionInfo? WindowsVersion { get; set; }
    public string? TimeZone { get; set; }
    public string? TimeZoneId { get; set; }
    public string? UtcOffset { get; set; }
    public CpuInfo? CPU { get; set; }
    public GpuInfo? GPU { get; set; }
    public RamInfo? RAM { get; set; }
    public List<DiskInfo>? Disks { get; set; }
    public List<NetworkAdapterInfo>? NetworkAdapters { get; set; }
    public WiFiInfo? WiFi { get; set; }
    public List<DriverInfo>? Drivers { get; set; }
    public List<ServiceInfo>? Services { get; set; }
    public string? ProductKey { get; set; }
    public string? Username { get; set; }
    public string? MicrosoftAccountEmail { get; set; }
    public MotherboardInfo? Motherboard { get; set; }
    public BiosInfo? BIOS { get; set; }
    public string? SystemUptime { get; set; }
    public List<SoftwareInfo>? InstalledSoftware { get; set; }
    public List<ProcessInfo>? RunningProcesses { get; set; }
    public List<StartupItemInfo>? StartupItems { get; set; }
    public List<HotfixInfo>? InstalledHotfixes { get; set; }
    public List<PrinterInfo>? Printers { get; set; }
    public List<AudioDeviceInfo>? AudioDevices { get; set; }
    public List<string>? HostsFileEntries { get; set; }
    public List<MappedDriveInfo>? MappedDrives { get; set; }
    public List<EnvironmentVariableInfo>? EnvironmentVariables { get; set; }
    public BatteryInfo? Battery { get; set; }
    public string? SystemBootTime { get; set; }
    public List<LocalUserInfo>? LocalUsers { get; set; }
    public List<string>? InstalledFonts { get; set; }
    public ActivationInfo? ActivationStatus { get; set; }
    public List<PortInfo>? ListeningPorts { get; set; }
    public List<EventLogEntryInfo>? EventLogErrors { get; set; }
    public List<WindowsFeatureInfo>? WindowsFeatures { get; set; }
    public List<ScheduledTaskInfo>? ScheduledTasks { get; set; }
    public List<NetworkShareInfo>? NetworkShares { get; set; }
    public List<ProcessDetailInfo>? ProcessDetails { get; set; }
    public List<CertificateInfo>? Certificates { get; set; }
    public List<UpdateHistoryInfo>? WindowsUpdateHistory { get; set; }
    public List<FirewallRuleInfo>? FirewallRules { get; set; }
    public List<UwpAppInfo>? UwpApps { get; set; }
    public List<BluetoothDeviceInfo>? BluetoothDevices { get; set; }
    public List<UsbDeviceInfo>? UsbDevices { get; set; }
    public List<DiskPartitionInfo>? DiskPartitions { get; set; }
    public LocaleInfo? SystemLocale { get; set; }
    public List<DotNetFrameworkInfo>? DotNetFrameworks { get; set; }
    public List<PowerSchemeInfo>? PowerSchemes { get; set; }
    public string? DirectXVersion { get; set; }
    public string? HardwareId { get; set; } // NEW
}

public class CpuInfo
{
    public string? Name { get; set; }
    public string? Manufacturer { get; set; }
    public string? MaxClockSpeed { get; set; }
    public string? Cores { get; set; }
    public string? LogicalProcessors { get; set; }
    public string? Architecture { get; set; }
    public string? ProcessorId { get; set; } // NEW
}

public class GpuInfo
{
    public string? Name { get; set; }
    public string? DriverVersion { get; set; }
    public string? DriverDate { get; set; }
    public string? AdapterRAM { get; set; }
}

public class RamInfo
{
    public string? TotalPhysical { get; set; }
    public string? FreePhysical { get; set; }
    public string? TotalVirtual { get; set; }
    public string? FreeVirtual { get; set; }
}

public class DiskInfo
{
    public string? DeviceID { get; set; }
    public string? VolumeName { get; set; }
    public string? FileSystem { get; set; }
    public string? Size { get; set; }
    public string? FreeSpace { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; } // NEW
}

public class NetworkAdapterInfo
{
    public string? Description { get; set; }
    public string? MAC { get; set; }
    public string? IPv4 { get; set; }
    public string? IPv6 { get; set; }
    public string? SubnetMask { get; set; }
    public string? Gateway { get; set; }
    public string? DNS { get; set; }
    public bool DHCPEnabled { get; set; }
}

public class WiFiInfo
{
    public string? SSID { get; set; }
    public string? BSSID { get; set; }
    public string? SignalQuality { get; set; }
    public string? RadioType { get; set; }
    public string? Security { get; set; }
}

public class WindowsVersionInfo
{
    public string? OS { get; set; }
    public string? Edition { get; set; }
    public string? Build { get; set; }
    public string? ProductName { get; set; }
    public string? InstallDate { get; set; }
}

public class DriverInfo
{
    public string? DeviceName { get; set; }
    public string? DriverVersion { get; set; }
    public string? DriverDate { get; set; }
    public string? InfName { get; set; }
    public string? HardwareID { get; set; }
}

public class ServiceInfo
{
    public string? DisplayName { get; set; }
    public string? ServiceName { get; set; }
    public string? Status { get; set; }
    public string? StartType { get; set; }
}

public class MotherboardInfo
{
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? Version { get; set; }
}

public class BiosInfo
{
    public string? Manufacturer { get; set; }
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? SerialNumber { get; set; }
    public string? ReleaseDate { get; set; }
}

public class SoftwareInfo
{
    public string? DisplayName { get; set; }
    public string? DisplayVersion { get; set; }
    public string? Publisher { get; set; }
    public string? InstallDate { get; set; }
}

public class ProcessInfo
{
    public string? Name { get; set; }
    public int PID { get; set; }
    public string? MemoryMB { get; set; }
    public string? TotalProcessorTime { get; set; }
}

public class StartupItemInfo
{
    public string? Name { get; set; }
    public string? Command { get; set; }
    public string? Location { get; set; }
}

public class HotfixInfo
{
    public string? HotFixID { get; set; }
    public string? Description { get; set; }
    public string? InstallDate { get; set; }
}

public class PrinterInfo
{
    public string? Name { get; set; }
    public string? DriverName { get; set; }
    public string? PortName { get; set; }
    public string? Status { get; set; }
}

public class AudioDeviceInfo
{
    public string? Name { get; set; }
    public string? Manufacturer { get; set; }
    public string? Status { get; set; }
}

public class MappedDriveInfo
{
    public string? DeviceID { get; set; }
    public string? UNC { get; set; }
    public string? FileSystem { get; set; }
    public string? Size { get; set; }
    public string? FreeSpace { get; set; }
}

public class EnvironmentVariableInfo
{
    public string? Name { get; set; }
    public string? Value { get; set; }
    public string? Target { get; set; }
}

public class BatteryInfo
{
    public string? Name { get; set; }
    public string? Manufacturer { get; set; }
    public string? EstimatedChargeRemaining { get; set; }
    public string? Status { get; set; }
    public string? Chemistry { get; set; }
    public string? DesignCapacity { get; set; }
}

public class LocalUserInfo
{
    public string? Name { get; set; }
    public string? FullName { get; set; }
    public string? Domain { get; set; }
    public string? SID { get; set; }
    public string? Status { get; set; }
}

public class ActivationInfo
{
    public string? LicenseStatus { get; set; }
    public string? ProductID { get; set; }
    public string? PartialProductKey { get; set; }
    public string? RemainingGracePeriod { get; set; }
}

public class PortInfo
{
    public string? Protocol { get; set; }
    public string? Address { get; set; }
    public int Port { get; set; }
    public string? State { get; set; }
    public int? ProcessId { get; set; }
}

public class EventLogEntryInfo
{
    public string? LogName { get; set; }
    public string? Time { get; set; }
    public string? Source { get; set; }
    public long EventID { get; set; }
    public string? Message { get; set; }
}

public class WindowsFeatureInfo
{
    public string? Name { get; set; }
    public string? InstallState { get; set; }
}

public class ScheduledTaskInfo
{
    public string? Name { get; set; }
    public string? Path { get; set; }
    public string? State { get; set; }
    public bool Enabled { get; set; }
    public string? LastRunTime { get; set; }
    public string? NextRunTime { get; set; }
    public string? Description { get; set; }
}

public class NetworkShareInfo
{
    public string? Name { get; set; }
    public string? Path { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
}

public class ProcessDetailInfo
{
    public string? Name { get; set; }
    public int PID { get; set; }
    public string? CommandLine { get; set; }
    public string? ExecutablePath { get; set; }
}

public class CertificateInfo
{
    public string? Subject { get; set; }
    public string? Issuer { get; set; }
    public string? SerialNumber { get; set; }
    public string? Thumbprint { get; set; }
    public string? NotBefore { get; set; }
    public string? NotAfter { get; set; }
    public string? StoreLocation { get; set; }
}

public class UpdateHistoryInfo
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Date { get; set; }
    public string? Operation { get; set; }
    public string? ResultCode { get; set; }
}

public class FirewallRuleInfo
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Direction { get; set; }
    public string? Action { get; set; }
    public string? Protocol { get; set; }
    public string? LocalPorts { get; set; }
    public string? RemotePorts { get; set; }
    public string? LocalAddresses { get; set; }
    public string? RemoteAddresses { get; set; }
    public bool Enabled { get; set; }
}

public class UwpAppInfo
{
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? Publisher { get; set; }
    public string? Architecture { get; set; }
    public string? PackageFamilyName { get; set; }
}

public class BluetoothDeviceInfo
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Status { get; set; }
    public bool Connected { get; set; }
}

public class UsbDeviceInfo
{
    public string? Name { get; set; }
    public string? DeviceID { get; set; }
    public string? Manufacturer { get; set; }
    public string? Status { get; set; }
}

public class DiskPartitionInfo
{
    public string? Name { get; set; }
    public string? DiskIndex { get; set; }
    public string? Index { get; set; }
    public string? Type { get; set; }
    public string? SizeGB { get; set; }
    public string? StartingOffset { get; set; }
    public bool Bootable { get; set; }
}

public class LocaleInfo
{
    public string? CurrentCulture { get; set; }
    public string? CurrentUICulture { get; set; }
    public string? InstalledUICulture { get; set; }
    public string? DisplayName { get; set; }
    public string? EnglishName { get; set; }
}

public class DotNetFrameworkInfo
{
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? Release { get; set; }
    public string? Type { get; set; }
}

public class PowerSchemeInfo
{
    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}