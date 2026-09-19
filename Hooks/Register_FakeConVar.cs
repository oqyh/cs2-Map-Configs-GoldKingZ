using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Cvars;

namespace Map_Configs_GoldKingZ;

public static class FakeConVar
{
    public static FakeConVar<string> LoadPlugin = new("gkz_load", "Load Or Reload A Plugin Name (Checks Disabled Folder)", "");
    public static FakeConVar<string> UnLoadPlugin = new("gkz_unload", "Unload A Plugin Name", "");
    public static FakeConVar<bool> UnLoadAllPlugins = new("gkz_unloadall", "Unload Every Plugin", false);
    public static FakeConVar<bool> RemoveMapServerCommands = new("gkz_removemapservercommands", "Remove Map Server Commands", false);
    public static FakeConVar<bool> RemoveMapClientCommands = new("gkz_removemapclientcommands", "Remove Map Client Commands", false);
    public static FakeConVar<bool> ForceRestart = new("gkz_forcerestart", "Force Restart Map", false);
    public static FakeConVar<string> ExecCfg = new("gkz_exec", "Execute A Cfg From csgo/cfg/", "");
}

public partial class MainPlugin
{
    public void Register_FakeConVar()
    {
        RegisterFakeConVars(typeof(FakeConVar));

        Remove_FakeConVar();

        FakeConVar.LoadPlugin.ValueChanged       += OnLoadRequested;
        FakeConVar.UnLoadPlugin.ValueChanged     += OnUnloadRequested;
        FakeConVar.UnLoadAllPlugins.ValueChanged += OnUnloadAllRequested;
        FakeConVar.RemoveMapServerCommands.ValueChanged += OnRemoveMapServerCommandsRequested;
        FakeConVar.RemoveMapClientCommands.ValueChanged += OnRemoveMapClientCommandsRequested;
        FakeConVar.ForceRestart.ValueChanged += OnForceRestartRequested;
        FakeConVar.ExecCfg.ValueChanged += OnExecCfgRequested;
    }

    public void Remove_FakeConVar()
    {
        FakeConVar.LoadPlugin.ValueChanged       -= OnLoadRequested;
        FakeConVar.UnLoadPlugin.ValueChanged     -= OnUnloadRequested;
        FakeConVar.UnLoadAllPlugins.ValueChanged -= OnUnloadAllRequested;
        FakeConVar.RemoveMapServerCommands.ValueChanged -= OnRemoveMapServerCommandsRequested;
        FakeConVar.RemoveMapClientCommands.ValueChanged -= OnRemoveMapClientCommandsRequested;
        FakeConVar.ForceRestart.ValueChanged -= OnForceRestartRequested;
        FakeConVar.ExecCfg.ValueChanged -= OnExecCfgRequested;
    }

    private static void OnLoadRequested(object? sender, string value)
    {
        if(string.IsNullOrWhiteSpace(value))return;

        Helper.LoadPlugin(value);
    }

    private static void OnUnloadRequested(object? sender, string value)
    {
        if(string.IsNullOrWhiteSpace(value))return;

        Helper.UnloadPlugin(value);
    }

    private static void OnUnloadAllRequested(object? sender, bool value)
    {
        if (!value)return;

        Helper.UnloadAllPlugins();
    }
    private static void OnRemoveMapServerCommandsRequested(object? sender, bool value)
    {
        if (!value) return;
        RemoveExistingCommands("point_servercommand");
    }

    private static void OnRemoveMapClientCommandsRequested(object? sender, bool value)
    {
        if (!value) return;
        RemoveExistingCommands("point_clientcommand");
    }

    private static void OnForceRestartRequested(object? sender, bool value)
    {
        if (!value)return;

        if(!string.IsNullOrWhiteSpace(Globals.Last_MapRestarted) && !string.IsNullOrWhiteSpace(Server.MapName))
        {
            if(Globals.Last_MapRestarted == Server.MapName)
            {
                Globals.Last_MapRestarted = "";
                return;
            }
        }

        Instance.AddTimer(3.0f, ()=>
        {
            Helper.Debug("[gkz_forcerestart] Restarting Map");
            Globals.Last_MapRestarted = Server.MapName;
            Helper.RestartMap();
        }, CounterStrikeSharp.API.Modules.Timers.TimerFlags.STOP_ON_MAPCHANGE);
    }

    private static void OnExecCfgRequested(object? sender, string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;

        Helper.ExecCfgNow(value);
    }
}