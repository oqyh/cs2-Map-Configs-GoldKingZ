using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;

namespace Map_Configs_GoldKingZ;

public static class Globals
{
    public static List<(string Name, string Value)> HookCommands = new();
    public static Dictionary<string, string> HookConVars = new(StringComparer.OrdinalIgnoreCase);
    public class PlayerDataClass
    {
        public CCSPlayerController? Player { get; set; }
        public DateTime EventPlayerChat { get; set; } = DateTime.MinValue;
    }
    public static readonly Dictionary<int, PlayerDataClass> Player_Data = new();
    public static string Last_MapRestarted = "";
    public static int Last_GameMode = -1;
    public static int Last_GameType  = -1;
    public static void Clear()
    {
        Player_Data?.Clear();
        FakeConVar.LoadPlugin.Value = "";
        FakeConVar.UnLoadPlugin.Value = "";
        FakeConVar.UnLoadAllPlugins.Value = false;
        FakeConVar.RemoveMapServerCommands.Value = false;
        FakeConVar.RemoveMapClientCommands.Value = false;
        FakeConVar.ForceRestart.Value = false;
    }

    public static void GetInfo()
    {
        var gmCvar = ConVar.Find("game_mode");
        if (gmCvar != null)
        {
            var gm = gmCvar.GetPrimitiveValue<int>();
            Last_GameMode = gm;
        }

        var gtCvar = ConVar.Find("game_type");
        if (gtCvar != null)
        {
            var gt = gtCvar.GetPrimitiveValue<int>();
            Last_GameType = gt;
        }
    }

}
