using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WinISA.Helpers;

public class WlanClient : IDisposable
{
    private IntPtr _clientHandle;
    private uint _negotiatedVersion;

    public WlanClient()
    {
        WlanOpenHandle(2, IntPtr.Zero, out _negotiatedVersion, out _clientHandle);
    }

    public IEnumerable<WlanInterface> EnumerateInterfaces()
    {
        WlanEnumInterfaces(_clientHandle, IntPtr.Zero, out var ifaceList);
        var size = Marshal.SizeOf(typeof(WlanInterfaceInfo));
        var ptr = ifaceList.InterfaceInfo;
        for (int i = 0; i < ifaceList.NumberOfItems; i++)
        {
            var info = Marshal.PtrToStructure<WlanInterfaceInfo>(ptr);
            yield return new WlanInterface(_clientHandle, info);
            ptr += size;
        }
    }

    public void Dispose()
    {
        if (_clientHandle != IntPtr.Zero)
            WlanCloseHandle(_clientHandle, IntPtr.Zero);
    }

    [DllImport("wlanapi.dll")]
    private static extern uint WlanOpenHandle(uint clientVersion, IntPtr reserved, out uint negotiatedVersion, out IntPtr clientHandle);

    [DllImport("wlanapi.dll")]
    private static extern uint WlanEnumInterfaces(IntPtr clientHandle, IntPtr reserved, out WlanInterfaceInfoList interfaceList);

    [DllImport("wlanapi.dll")]
    private static extern uint WlanCloseHandle(IntPtr clientHandle, IntPtr reserved);

    [DllImport("wlanapi.dll")]
    private static extern uint WlanQueryInterface(IntPtr clientHandle, ref Guid interfaceGuid, WlanIntfOpcode opcode,
        IntPtr reserved, out uint dataSize, out IntPtr data, out WlanOpcodeValueType valueType);

    private enum WlanIntfOpcode { CurrentConnection = 8 }
    private enum WlanOpcodeValueType { Ignore }

    [StructLayout(LayoutKind.Sequential)]
    private struct WlanInterfaceInfoList
    {
        public uint NumberOfItems;
        public int Index;
        public IntPtr InterfaceInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WlanInterfaceInfo
    {
        public Guid InterfaceGuid;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string InterfaceDescription;
        public WlanInterfaceState isState;
    }

    public enum WlanInterfaceState
    {
        NotReady = 0,
        Connected = 1,
        AdHocNetworkFormed = 2,
        Disconnecting = 3,
        Disconnected = 4,
        Associating = 5,
        Discovering = 6,
        Authenticating = 7
    }

    public class WlanInterface
    {
        private readonly IntPtr _clientHandle;
        private readonly WlanInterfaceInfo _info;

        public WlanInterface(IntPtr clientHandle, WlanInterfaceInfo info)
        {
            _clientHandle = clientHandle;
            _info = info;
        }

        public WlanConnectionAttributes CurrentConnection
        {
            get
            {
                var guid = _info.InterfaceGuid;
                WlanQueryInterface(_clientHandle, ref guid, WlanIntfOpcode.CurrentConnection,
                    IntPtr.Zero, out _, out var data, out _);
                var conn = Marshal.PtrToStructure<WlanConnectionAttributes>(data);
                Marshal.FreeHGlobal(data);
                return conn;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WlanConnectionAttributes
    {
        public WlanInterfaceState isState;
        public Dot11Ssid dot11Ssid;
        public Dot11BssType dot11BssType;
        public Dot11AuthAlgorithm dot11AuthAlgorithm;
        public Dot11CipherAlgorithm dot11CipherAlgorithm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] bssid;
        public uint wlanSignalQuality;
        public uint ulRxRate;
        public uint ulTxRate;
        public Dot11PhyType dot11PhyType;
        public uint uLinkQuality;
        public bool bSecurityEnabled;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Dot11Ssid
    {
        public uint SSIDLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] SSID;
    }

    public enum Dot11BssType { Infrastructure = 1, Independent = 2, Any = 3 }
    public enum Dot11AuthAlgorithm
    {
        IEEE80211_Open = 0,
        IEEE80211_Shared = 1,
        WPA = 2,
        WPA_PSK = 3,
        WPA_None = 4,
        RSNA = 5,
        RSNA_PSK = 6
    }
    public enum Dot11CipherAlgorithm { None = 0, WEP40 = 1, TKIP = 2, CCMP = 4, WEP104 = 5, BIP = 6 }
    public enum Dot11PhyType { FHSS = 1, DSSS = 2, IRBaseband = 3, OFDM = 4, HRDSSS = 5, ERP = 6, HT = 7, VHT = 8 }
}