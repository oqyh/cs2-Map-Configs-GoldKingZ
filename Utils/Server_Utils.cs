using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;

namespace Map_Configs_GoldKingZ;

public static class Server_Utils
{
    private static readonly IntPtr _networkServerService;
    private static readonly IntPtr _networkSystem;

    private delegate IntPtr GetGameServerHandle(IntPtr networkServerService);
    private delegate IntPtr GetWorkshopId(IntPtr gameServer);
    private delegate nint UpdatePublicIp(IntPtr networkSystem);
    private delegate IntPtr PortFromHandle(IntPtr self, uint handle);

    private static readonly GetGameServerHandle _getGameServer;
    private static readonly GetWorkshopId _getWorkshopId;
    private static UpdatePublicIp? _updatePublicIp;

    private const int UPDATE_PUBLIC_IP = 256;
    private const int PORT_INDEX = 34;
    private const int HANDLE_OFFSET = 0x40;

    private static string _cachedIp = "";

    static unsafe Server_Utils()
    {
        _networkServerService = NativeAPI.GetValveInterface(0, "NetworkServerService_001");
        _networkSystem = NativeAPI.GetValveInterface(0, "NetworkSystemVersion001");

        int off = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? 24 : 23;
        _getGameServer = Marshal.GetDelegateForFunctionPointer<GetGameServerHandle>(
            *(*(IntPtr**)_networkServerService + off));

        var server = _getGameServer(_networkServerService);
        _getWorkshopId = Marshal.GetDelegateForFunctionPointer<GetWorkshopId>(
            *(*(IntPtr**)server + 26));
    }
    public static unsafe string GetIp()
    {
        if (!string.IsNullOrEmpty(_cachedIp)) return _cachedIp;
        if (_networkSystem == IntPtr.Zero) return "";

        if (_updatePublicIp == null)
        {
            var fp = *(nint*)(*(nint*)_networkSystem + UPDATE_PUBLIC_IP);
            if (fp == 0) return "";
            _updatePublicIp = Marshal.GetDelegateForFunctionPointer<UpdatePublicIp>(fp);
        }

        nint p = _updatePublicIp(_networkSystem);
        if (p == 0) return "";

        var b = (byte*)(p + 4);
        string ip = $"{b[0]}.{b[1]}.{b[2]}.{b[3]}";

        if (ip.StartsWith("0.")) return "";
        return _cachedIp = ip;
    }

    public static unsafe int GetPort()
    {
        try
        {
            var server = _getGameServer(_networkServerService);
            if (server != IntPtr.Zero && _networkSystem != IntPtr.Zero)
            {
                uint handle = (uint)Marshal.ReadInt32(server, HANDLE_OFFSET);
                var fn = Marshal.GetDelegateForFunctionPointer<PortFromHandle>(
                    *(*(IntPtr**)_networkSystem + PORT_INDEX));

                int p = (int)(fn(_networkSystem, handle).ToInt64() & 0xFFFF);
                if (p > 0) return p;
            }
        }
        catch { }

        int cv = ConVar.Find("hostport")?.GetPrimitiveValue<int>() ?? 0;
        return cv > 0 ? cv : 27015;
    }

    public static string GetServerIp()
    {
        string ip = GetIp();
        return ip.Length == 0 ? "" : $"{ip}:{GetPort()}";
    }
    public static string GetAddonID()
    {
        var server = _getGameServer(_networkServerService);
        var str = Marshal.PtrToStringAnsi(_getWorkshopId(server));

        if (string.IsNullOrEmpty(str)) return "";

        string id = str.Split(',')[0].Trim();
        return GetExtraAddonsList().Contains(id) ? "" : id;
    }

    private static List<string> GetExtraAddonsList()
    {
        var list = ParseIds(ConVar.Find("mm_extra_addons")?.StringValue ?? "");
        if (list.Count > 0) return list;

        try
        {
            string path = GetConfigFilePath();
            if (!string.IsNullOrEmpty(path))
            {
                var m = Regex.Match(File.ReadAllText(path), @"mm_extra_addons\s+""([^""]*)""");
                if (m.Success) return ParseIds(m.Groups[1].Value);
            }
        }
        catch { }

        return new List<string>();
    }

    private static List<string> ParseIds(string input)
    {
        if (string.IsNullOrEmpty(input)) return new List<string>();

        return input.Split(',')
            .Select(x => x.Trim())
            .Where(x => x.Length > 0 && x.All(char.IsDigit))
            .ToList();
    }

    private static string GetConfigFilePath()
    {
        try
        {
            var gameDir = new DirectoryInfo(MainPlugin.Instance.ModuleDirectory)
                .Parent?.Parent?.Parent?.Parent;
            if (gameDir == null || !gameDir.Exists) return null!;

            string path = Path.Combine(gameDir.FullName, "cfg", "multiaddonmanager", "multiaddonmanager.cfg");
            return File.Exists(path) ? path : null!;
        }
        catch { return null!; }
    }
}