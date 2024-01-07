using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace Map_Configs_Prefix;

public class MapConfigsPrefixConfig : BasePluginConfig
{
    [JsonPropertyName("ConVarEnforcer")] public bool ConVarEnforcer { get; set; } = false;
    [JsonPropertyName("EnableErrorLogChecker")] public bool EnableErrorLogChecker { get; set; } = false;
}

public class MapConfigsPrefix : BasePlugin, IPluginConfig<MapConfigsPrefixConfig>
{
    public override string ModuleName => "Map Configs Prefix";
    public override string ModuleVersion => "1.0.1";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "Map Configs Depend Map Name";
    public MapConfigsPrefixConfig Config { get; set; } = new MapConfigsPrefixConfig();
    private string Tpath = "";
    private string Date = "";
    public static string SMapName => NativeAPI.GetMapName();
    public void OnConfigParsed(MapConfigsPrefixConfig config)
    {
        Config = config; 
    }
    
    public override void Load(bool hotReload)
    {
        ExecCommandMap();

        RegisterListener<Listeners.OnMapStart>(OnMapStartHandler);

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
    }
    private void ExecCommandMap()
    {
        string folderPath = Path.Combine(ModuleDirectory, "../../plugins/Map_Configs_Prefix");
        if(SMapName == null)return;
        int underscoreIndex = SMapName.IndexOf('_');
        string result = underscoreIndex != -1 ? SMapName.Substring(0, underscoreIndex + 1) : SMapName;

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
                    if (!string.IsNullOrEmpty(shortFileName))
                    {
                        if(result.Equals(shortFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            foundMatch = true;
                            Server.ExecuteCommand($"exec Map-Configs-Prefix/{shortFileName}");
                            break;
                        }else if(SMapName.Equals(shortFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            foundMatch = true;
                            Server.ExecuteCommand($"exec Map-Configs-Prefix/{shortFileName}");
                            break;
                        }
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
                        Server.ExecuteCommand($"exec Map-Configs-Prefix/{defaultFileName}");
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
    private void OnMapStartHandler(string mapName)
    {
        ExecCommandMap();
        Server.NextFrame(() =>
        {
            ExecCommandMap();
            AddTimer(2.0f, () =>
            {
                ExecCommandMap();
            });
            AddTimer(3.0f, () =>
            {
                ExecCommandMap();
            });
            AddTimer(4.0f, () =>
            {
                ExecCommandMap();
            });
        });
        
    }

    [GameEventHandler(HookMode.Post)]
    public HookResult OnEventRoundStartPost(EventRoundStart @event, GameEventInfo info)
    {
        if(Config.ConVarEnforcer)
        {
            ExecCommandMap();
            Server.NextFrame(() =>
            {
                ExecCommandMap();
                AddTimer(2.0f, () =>
                {
                    ExecCommandMap();
                });
                AddTimer(3.0f, () =>
                {
                    ExecCommandMap();
                });
            });
        }
        return HookResult.Continue;
    }
}