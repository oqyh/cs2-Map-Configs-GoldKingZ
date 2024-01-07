using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Timers;

namespace Map_Configs_Prefix;

public class MapConfigsPrefixConfig : BasePluginConfig
{
    [JsonPropertyName("ForceCvar")] public bool ForceCvar { get; set; } = false;
    [JsonPropertyName("EnableErrorLogChecker")] public bool EnableErrorLogChecker { get; set; } = false;
}

public class MapConfigsPrefix : BasePlugin, IPluginConfig<MapConfigsPrefixConfig>
{
    public override string ModuleName => "Map Configs Prefix";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "Map Configs Depend Map Name";
    public MapConfigsPrefixConfig Config { get; set; } = new MapConfigsPrefixConfig();
    public static string MapName => NativeAPI.GetMapName();

    private bool onetime;
    private string Tpath = "";
    private string Date = "";
    private CounterStrikeSharp.API.Modules.Timers.Timer? _forcecvar;
    public void OnConfigParsed(MapConfigsPrefixConfig config)
    {
        Config = config; 
    }
    
    public override void Load(bool hotReload)
    {
        string Fpath = Path.Combine(ModuleDirectory,"../../plugins/Map_Configs_Prefix/ErrorLogs/");
        Date = DateTime.Now.ToString("MM-dd-yyyy");
        string fileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
        Tpath = Path.Combine(ModuleDirectory,"../../plugins/Map_Configs_Prefix/ErrorLogs/") + $"{fileName}";

        if(Config.EnableErrorLogChecker && !Directory.Exists(Fpath))
        {
            Directory.CreateDirectory(Fpath);
        }

        if(Config.EnableErrorLogChecker && !File.Exists(Tpath))
        {
            File.Create(Tpath);
        }

        onetime = false;
        ExecCommandMap();
        if(Config.ForceCvar)
        {
            _forcecvar?.Kill();
            _forcecvar = null;
            _forcecvar = AddTimer(0.1f, forcecvarTimer_Callback, TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
        }
    }
    private void forcecvarTimer_Callback()
    {
        ExecCommandMap();
    }

    [GameEventHandler(HookMode.Pre)]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(Config.ForceCvar)
        {
            _forcecvar?.Kill();
            _forcecvar = null;
            _forcecvar = AddTimer(0.1f, forcecvarTimer_Callback, TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
        }
        if(onetime == false)
        {
            ExecCommandMap();
            Server.NextFrame(() =>
            {
                ExecCommandMap();
                AddTimer(3.0f, () =>
                {
                    ExecCommandMap();
                    onetime = true;
                });
            });
        }
        return HookResult.Continue;
    }

    private void ExecCommandMap()
    {
        string folderPath = Path.Combine(ModuleDirectory, "../../plugins/Map_Configs_Prefix");

        int underscoreIndex = MapName.IndexOf('_');
        string result = underscoreIndex != -1 ? MapName.Substring(0, underscoreIndex + 1) : MapName;

        for (int i = 0; i < 4; i++)
        {
            folderPath = Path.Combine(folderPath, "..");
        }
        
        string cfgDirectory = Path.Combine(folderPath, "cfg");
        if (Directory.Exists(cfgDirectory))
        {
            string mapsCfgDirectory = Path.Combine(cfgDirectory, "Map-Configs-Prefix");
            if (Directory.Exists(mapsCfgDirectory))
            {
                string[] fileNamess = Directory.GetFiles(mapsCfgDirectory);
                bool foundMatch = false;
                foreach (string fileName in fileNamess)
                {
                    string shortFileName = Path.GetFileNameWithoutExtension(fileName);
                    if (!string.IsNullOrEmpty(shortFileName) && 
                    (result.Equals(shortFileName, StringComparison.OrdinalIgnoreCase) || 
                    MapName.Equals(shortFileName, StringComparison.OrdinalIgnoreCase)))
                    {
                        Server.ExecuteCommand($"execifexists Map-Configs-Prefix/{shortFileName}");
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch)
                {
                    if (Config.EnableErrorLogChecker && File.Exists(Tpath))
                    {
                        try
                        {
                            File.AppendAllLines(Tpath, new[]{$"[{Date} - Couldn't Found Files in csgo/cfg/Map-Configs-Prefix/ That Match Current Map]"});
                        }catch
                        {
                            File.AppendAllLines(Tpath, new[]{$"[{Date} - Please Give Map_Configs_Prefix.dll Permissions To Write.]"});
                        }
                    }

                    string defaultFileName = "_default_.cfg";
                    string defaultFilePath = Path.Combine(mapsCfgDirectory, defaultFileName);
                    if (File.Exists(defaultFilePath))
                    {
                        Server.ExecuteCommand($"execifexists Map-Configs-Prefix/{defaultFileName}");
                    }else if (!File.Exists(defaultFilePath))
                    {
                        if (Config.EnableErrorLogChecker && File.Exists(Tpath))
                        {
                            try
                            {
                                File.AppendAllLines(Tpath, new[]{$"[{Date} - csgo/cfg/Map-Configs-Prefix/_default_.cfg file does not exist.]"});
                            }catch
                            {
                                File.AppendAllLines(Tpath, new[]{$"[{Date} - Please Give Map_Configs_Prefix.dll Permissions To Write.]"});
                            }
                        }
                    }
                }
            }
            else
            {
                if (Config.EnableErrorLogChecker && File.Exists(Tpath))
                {
                    try
                    {
                        File.AppendAllLines(Tpath, new[]{$"[{Date} - csgo/cfg/Map-Configs-Prefix/ directory does not exist.]"});
                    }catch
                    {
                        File.AppendAllLines(Tpath, new[]{$"[{Date} - Please Give Map_Configs_Prefix.dll Permissions To Write.]"});
                    }
                }
                
            }
        }
        else
        {
            if (Config.EnableErrorLogChecker && File.Exists(Tpath))
            {
                try
                {
                    File.AppendAllLines(Tpath, new[]{$"[{Date} - csgo/cfg/ directory does not exist.]"});
                }catch
                {
                    File.AppendAllLines(Tpath, new[]{$"[{Date} - Please Give Map_Configs_Prefix.dll Permissions To Write.]"});
                }
            }
        }
    }
        
}