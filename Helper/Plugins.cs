using System.Reflection;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Plugin;
using CounterStrikeSharp.API.Core.Plugin.Host;
using CounterStrikeSharp.API.Modules.Cvars;

namespace Map_Configs_GoldKingZ;

public partial class Helper
{
    public static string PluginsRoot => Path.GetDirectoryName(Path.GetDirectoryName(MainPlugin.Instance.ModulePath))!;
    public static IEnumerable<PluginContext> GetLoadedPlugins() => Manager()?.GetLoadedPlugins() ?? Enumerable.Empty<PluginContext>();
    public static string FolderOf(PluginContext p) => string.IsNullOrEmpty(p.FilePath) ? "" : Path.GetFileNameWithoutExtension(p.FilePath);
    public static PluginContext? Find(string name) => GetLoadedPlugins().FirstOrDefault(p => FolderOf(p).Equals(name, StringComparison.OrdinalIgnoreCase));
    public static string Clean(string v) => v.Trim().Trim('"').Trim();
    public static bool IsSelf(PluginContext ctx) => ReferenceEquals(ctx.Plugin, MainPlugin.Instance);
    public static bool IsTrue(string v) => v.Equals("1", StringComparison.OrdinalIgnoreCase) || v.Equals("true", StringComparison.OrdinalIgnoreCase);
    
    public static IPluginManager? Manager()
    {
        var app = Application.Instance;
        if (app == null) return null;

        var field = typeof(Application)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(f => typeof(IPluginManager).IsAssignableFrom(f.FieldType));

        return field?.GetValue(app) as IPluginManager;
    }

    public static void SafeOnAllPluginsLoaded(PluginContext ctx, bool hotReload)
    {
        try
        {
            ctx.Plugin?.OnAllPluginsLoaded(hotReload);
        }
        catch (Exception e)
        {
            Debug($"OnAllPluginsLoaded Threw For \"{ctx.Plugin?.ModuleName ?? "unknown"}\" (Plugin Is Still Loaded): {e.Message}", true);
        }
    }

    public static bool EnsureEnabled(string name)
    {
        var enabledDir  = Path.Combine(PluginsRoot, name);
        var disabledDir = Path.Combine(PluginsRoot, "disabled", name);

        if (File.Exists(Path.Combine(enabledDir, name + ".dll"))) return true;

        if (Directory.Exists(enabledDir) || !File.Exists(Path.Combine(disabledDir, name + ".dll")))
        {
            Debug($"\"{name}\" Not Found In plugins/ Or plugins/disabled/", true);
            return false;
        }

        try
        {
            Directory.Move(disabledDir, enabledDir);
            return true;
        }
        catch (Exception e)
        {
            Debug($"Could Not Enable \"{name}\": {e.Message}", true);
            return false;
        }
    }

    public static void LoadPlugin(string name)
    {
        name = Clean(name);
        if (name.Length == 0) return;

        var manager = Manager();
        if (manager == null)
        {
            Debug("Plugin Manager Unavailable", true);
            return;
        }

        var ctx = Find(name);

        if (ctx != null)
        {
            if (ctx.State == PluginState.Loaded) return;

            try
            {
                ctx.Unload(true);
                ctx.Load(true);
            }
            catch (Exception e)
            {
                Debug($"Failed To Reload \"{name}\": {e.Message}", true);
                return;
            }

            SafeOnAllPluginsLoaded(ctx, true);
            Debug($"Reloaded [#{ctx.PluginId}] \"{ctx.Plugin?.ModuleName ?? name}\"");
            return;
        }

        if (!EnsureEnabled(name)) return;

        try
        {
            manager.LoadPlugin(Path.Combine(PluginsRoot, name, name + ".dll"));
        }
        catch (Exception e)
        {
            Debug($"Failed To Load \"{name}\": {e.Message}", true);
            return;
        }

        ctx = Find(name);

        if (ctx?.Plugin == null)
        {
            Debug($"Failed To Load \"{name}\"", true);
            return;
        }

        SafeOnAllPluginsLoaded(ctx, false);
        Debug($"Loaded [#{ctx.PluginId}] \"{ctx.Plugin.ModuleName}\"");
    }

    public static void UnloadPlugin(string name)
    {
        name = Clean(name);
        if (name.Length == 0) return;

        var ctx = Find(name);
        if (ctx == null || ctx.State != PluginState.Loaded) return;

        if (IsSelf(ctx))
        {
            Debug("Refusing To Unload Myself", true);
            return;
        }

        UnloadOne(ctx);
    }

    public static void UnloadAllPlugins(IEnumerable<string>? keep = null)
    {
        var keepSet = new HashSet<string>(keep ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);

        foreach (var ctx in GetLoadedPlugins().ToList())
        {
            if (ctx.State != PluginState.Loaded) continue;
            if (IsSelf(ctx)) continue;
            if (keepSet.Contains(FolderOf(ctx))) continue;

            UnloadOne(ctx);
        }
    }

    public static void UnloadOne(PluginContext ctx)
    {
        var display = ctx.Plugin?.ModuleName ?? FolderOf(ctx);

        try
        {
            ctx.Unload(false);
            Debug($"Unloaded [#{ctx.PluginId}] \"{display}\"");
        }
        catch (Exception e)
        {
            Debug($"Failed To Unload \"{display}\": {e.Message}", true);
        }
    }

    public static void ApplyMapCfg(string mapName)
    {
        Globals.HookConVars.Clear();
        Globals.HookCommands.Clear();

        foreach (var file in mapName.ResolveMapCfgPaths())
        {
            LoadCfgIntoHookConVars(file);
        }

        ApplyHookConVars();
    }

    public static void LoadCfgIntoHookConVars(string? file)
    {
        if (file == null || !File.Exists(file)) return;

        var count = 0;

        foreach (var raw in File.ReadAllLines(file))
        {
            var line = raw.Trim();

            var comment = line.IndexOf("//", StringComparison.Ordinal);
            if (comment >= 0) line = line[..comment].Trim();

            if (line.Length == 0 || line[0] == ';') continue;

            var space = line.IndexOfAny(new[] { ' ', '\t' });
            if (space < 0) continue;

            var name  = line[..space].Trim();
            var value = line[(space + 1)..].Trim().Trim('"').Trim();

            if (name.Length == 0 || value.Length == 0) continue;

            if (name.Equals("gkz_exec", StringComparison.OrdinalIgnoreCase))
            {
                LoadExecCfg(value);
                continue;
            }

            if (name.StartsWith("gkz_", StringComparison.OrdinalIgnoreCase))
                Globals.HookCommands.Add((name.ToLowerInvariant(), value));
            else
                Globals.HookConVars[name] = value;

            count++;
        }

        Debug($"Loaded {count} Line(s) From \"{Path.GetFileName(file)}\"");
    }

    public static void ApplyHookConVars()
    {
        SetConVars(Globals.HookConVars);
        RunHookCommands(Globals.HookCommands);
    }

    public static void SetConVars(IEnumerable<KeyValuePair<string, string>> convars)
    {
        foreach (var (name, value) in convars)
        {
            var cvar = ConVar.Find(name);
            if (cvar == null)
            {
                Debug($"ConVar \"{name}\" Not Found, Skipped", true);
                continue;
            }

            try
            {
                cvar.StringValue = value;
                Debug($"Set {name} = {value}");
            }
            catch (Exception ex)
            {
                Debug($"Failed To Set {name} = {value}: {ex.Message}", true);
            }
        }
    }

    public static void RunHookCommands(IEnumerable<(string Name, string Value)> commands)
    {
        var loads     = new List<string>();
        var unloadAll = false;
        var others    = new List<(string Name, string Value)>();

        foreach (var (name, value) in commands)
        {
            switch (name)
            {
                case "gkz_load":
                    var n = Clean(value);
                    if (n.Length > 0 && !loads.Contains(n, StringComparer.OrdinalIgnoreCase)) loads.Add(n);
                    break;

                case "gkz_unloadall":
                    unloadAll = IsTrue(value);
                    break;

                default:
                    others.Add((name, value));
                    break;
            }
        }

        if (unloadAll)
        {
            UnloadAllPlugins(loads);
            Debug($"Unloaded All Plugins Except: {string.Join(", ", loads)}");
        }

        foreach (var n in loads)
            LoadPlugin(n);

        foreach (var (name, value) in others)
        {
            Server.ExecuteCommand($"{name} {value}");
            Debug($"Fired {name} {value}");
        }
    }

    private static readonly HashSet<string> ExecChain = new(StringComparer.OrdinalIgnoreCase);
    public static string? ResolveExecPath(string value)
    {
        value = Clean(value).Replace('\\', '/').TrimStart('/');
        if (value.Length == 0) return null;
        if (!value.EndsWith(".cfg", StringComparison.OrdinalIgnoreCase)) value += ".cfg";

        var root = Path.GetFullPath(Path.Join(Server.GameDirectory, "csgo", "cfg"));
        var full = Path.GetFullPath(Path.Join(root, value));

        if (!full.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) return null;
        if (!File.Exists(full)) return null;

        return full;
    }

    public static void LoadExecCfg(string value)
    {
        var path = ResolveExecPath(value);
        if (path == null)
        {
            Debug($"[gkz_exec] \"{Clean(value)}\" Not Found In ../csgo/cfg/", true);
            return;
        }

        if (!ExecChain.Add(path))
        {
            Debug($"[gkz_exec] Loop Detected, Skipped \"{Path.GetFileName(path)}\"", true);
            return;
        }

        try
        {
            Debug($"   [FOUND] gkz_exec ==> {Clean(value)}");
            LoadCfgIntoHookConVars(path);
        }
        finally
        {
            ExecChain.Remove(path);
        }
    }
    
    public static void ExecCfgNow(string value)
    {
        var before   = new Dictionary<string, string>(Globals.HookConVars);
        var cmdStart = Globals.HookCommands.Count;

        LoadExecCfg(value);

        var changed = Globals.HookConVars.Where(kv => !before.TryGetValue(kv.Key, out var old) || old != kv.Value).ToList();
        var cmds    = Globals.HookCommands.Skip(cmdStart).ToList();

        SetConVars(changed);
        RunHookCommands(cmds);
    }

    public static void RestartMap()
    {
        var MapName = Server.MapName;
        var MapWorkShop_ID = Server_Utils.GetAddonID();

        if (!string.IsNullOrWhiteSpace(MapWorkShop_ID))
        {
            Server.ExecuteCommand("host_workshop_map " + MapWorkShop_ID);
        }
        else
        {
            Server.ExecuteCommand("changelevel " + MapName);
        }
    }
    
    public static void RestartOnGameModeAndTypeChanged()
    {        
        var gmCvar = ConVar.Find("game_mode");
        var gtCvar = ConVar.Find("game_type");

        if (gmCvar == null || gtCvar == null)
        {
            Debug("[Restart Game Mode/Type Changed] game_mode/game_type not found");
            return;
        }

        var gm = gmCvar.GetPrimitiveValue<int>();
        var gt = gtCvar.GetPrimitiveValue<int>();

        if (Globals.Last_GameMode == -1 && Globals.Last_GameType == -1)
        {
            return;
        }

        if (Globals.Last_GameMode == gm && Globals.Last_GameType == gt)
        {
            Debug($"[Restart Game Mode/Type Changed] Mode unchanged (mode={gm} type={gt})");
            return;
        }

        Debug($"[Restart Game Mode/Type Changed] Changed: mode {Globals.Last_GameMode}->{gm}, type {Globals.Last_GameType}->{gt}");
        Server.NextFrame(()=>
        {
            MainPlugin.Instance.AddTimer(6.0f, ()=>
            {
                RestartMap();
            }, CounterStrikeSharp.API.Modules.Timers.TimerFlags.STOP_ON_MAPCHANGE);
        });
    }

    public static bool ShouldRemove(string designerName) => designerName.ToLowerInvariant() switch
    {
        "point_servercommand" => FakeConVar.RemoveMapServerCommands.Value,
        "point_clientcommand" => FakeConVar.RemoveMapClientCommands.Value,
        _                     => false
    };
}