using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Core.Attributes;

namespace Map_Configs_Prefix;

[MinimumApiVersion(164)]
public class MapConfigsPrefixConfig : BasePluginConfig
{
    [JsonPropertyName("InvertPathMode")] public bool InvertPathMode { get; set; } = false;
    [JsonPropertyName("ExecMode")] public string ExecMode { get; set; } = "OnRoundStart,OnMapStart,OnMatchStart,OnWarmupStart";
    [JsonPropertyName("ExecXTimes")] public int ExecXTimes { get; set; } = 3;

    [JsonPropertyName("ForceExecMode")] public string ForceExecMode { get; set; } = "OnPlayerSpawn";
    [JsonPropertyName("ForceExecXTimes")] public int ForceExecXTimes { get; set; } = 3;
    [JsonPropertyName("EnableErrorLogChecker")] public bool EnableErrorLogChecker { get; set; } = false;
}

public class MapConfigsPrefix : BasePlugin, IPluginConfig<MapConfigsPrefixConfig>
{
    public override string ModuleName => "Map Configs Prefix";
    public override string ModuleVersion => "1.0.5";
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
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterEventHandler<EventRoundAnnounceMatchStart>(OnMatchStart);
        RegisterEventHandler<EventRoundAnnounceWarmup>(OnWarmupStart);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
    }

    private void OnMapStart(string Map)
    {
        if (Config.ExecMode.Contains("OnMapStart"))
        {
            int x = Config.ExecXTimes;
            ExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                        ExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }

        if (Config.ForceExecMode.Contains("OnMapStart"))
        {
            int x = Config.ForceExecXTimes;
            ForceExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                        ForceExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        

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
    private HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo _)
    {
        if(@event == null)return HookResult.Continue;

        if (Config.ExecMode.Contains("OnPlayerSpawn"))
        {
            int x = Config.ExecXTimes;
            ExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                        ExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        if (Config.ForceExecMode.Contains("OnPlayerSpawn"))
        {
            int x = Config.ForceExecXTimes;
            ForceExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                        ForceExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }

        return HookResult.Continue;
    }

    private HookResult OnMatchStart(EventRoundAnnounceMatchStart @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        if (Config.ExecMode.Contains("OnMatchStart"))
        {
            int x = Config.ExecXTimes;
            ExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                        ExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        if (Config.ForceExecMode.Contains("OnMatchStart"))
        {
            int x = Config.ForceExecXTimes;
            ForceExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                        ForceExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        
        return HookResult.Continue;
    }
    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        if (Config.ExecMode.Contains("OnRoundStart"))
        {
            int x = Config.ExecXTimes;
            ExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                        ExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        if (Config.ForceExecMode.Contains("OnRoundStart"))
        {
            int x = Config.ForceExecXTimes;
            ForceExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                        ForceExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        
        return HookResult.Continue;
    }

    private HookResult OnWarmupStart(EventRoundAnnounceWarmup @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        if (Config.ExecMode.Contains("OnWarmupStart"))
        {
            int x = Config.ExecXTimes;
            ExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                        ExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        if (Config.ForceExecMode.Contains("OnWarmupStart"))
        {
            int x = Config.ForceExecXTimes;
            ForceExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                        ForceExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        
        return HookResult.Continue;
    }

    private void ExecCommandMap()
    {
        string folderPath = Path.Combine(ModuleDirectory, "../../plugins/Map_Configs_Prefix");
        if(SMapName == null)return;
        int underscoreIndex = SMapName.IndexOf('_');
        int nextUnderscoreIndex = SMapName.IndexOf('_', underscoreIndex + 1);
        string prefix = underscoreIndex != -1 ? SMapName.Substring(0, underscoreIndex + 1) : SMapName;
        string prefix2 = underscoreIndex != -1 ? SMapName.Substring(0, nextUnderscoreIndex + 1) : SMapName;
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
                        if(Config.InvertPathMode == false)
                        {
                            bool found = prefix.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                            if(found)
                            {
                                foundMatch = true;
                                Server.ExecuteCommand($"exec Map-Configs-Prefix/{shortFileName}");
                                break;
                            }
                        }else if(Config.InvertPathMode == true)
                        {
                            bool found = SMapName.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                            if(found)
                            {
                                foundMatch = true;
                                Server.ExecuteCommand($"exec Map-Configs-Prefix/{shortFileName}");
                                break;
                            }
                        }
                        
                    }
                }
                if (!foundMatch)
                {
                    bool foundMatch2 = false;
                    foreach (string fileName in fileNamess)
                    {
                        
                        string shortFileName = Path.GetFileNameWithoutExtension(fileName);
                        if (!string.IsNullOrEmpty(shortFileName))
                        {
                            if(Config.InvertPathMode == false)
                            {
                                bool found = prefix2.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                if(found)
                                {
                                    foundMatch2 = true;
                                    Server.ExecuteCommand($"exec Map-Configs-Prefix/{shortFileName}");
                                    break;
                                }
                            }else if(Config.InvertPathMode == true)
                            {
                                bool found = prefix2.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                if(found)
                                {
                                    foundMatch2 = true;
                                    Server.ExecuteCommand($"exec Map-Configs-Prefix/{fileName}");
                                    break;
                                }
                            }
                            
                        }
                    }
                    if (!foundMatch2)
                    {
                        bool foundMatch3 = false;
                        foreach (string fileName in fileNamess)
                        {
                            
                            string shortFileName = Path.GetFileNameWithoutExtension(fileName);
                            if (!string.IsNullOrEmpty(shortFileName))
                            {
                                if(Config.InvertPathMode == false)
                                {
                                    bool found = SMapName.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                    if(found)
                                    {
                                        foundMatch3 = true;
                                        Server.ExecuteCommand($"exec Map-Configs-Prefix/{shortFileName}");
                                        break;
                                    }
                                }else if(Config.InvertPathMode == true)
                                {
                                    bool found = prefix.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                    if(found)
                                    {
                                        foundMatch3 = true;
                                        Server.ExecuteCommand($"exec Map-Configs-Prefix/{shortFileName}");
                                        break;
                                    }
                                }
                                
                            }
                        }
                        if (!foundMatch3)
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
    private void ForceExecCommandMap()
    {
        string folderPath = Path.Combine(ModuleDirectory, "../../plugins/Map_Configs_Prefix");
        if(SMapName == null)return;
        int underscoreIndex = SMapName.IndexOf('_');
        int nextUnderscoreIndex = SMapName.IndexOf('_', underscoreIndex + 1);
        string prefix = underscoreIndex != -1 ? SMapName.Substring(0, underscoreIndex + 1) : SMapName;
        string prefix2 = underscoreIndex != -1 ? SMapName.Substring(0, nextUnderscoreIndex + 1) : SMapName;

        string Fresul = "f_" + prefix;
        string Fresul2 = "f_" + prefix2;
        string Fmap = "f_" + SMapName;

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
                        if(Config.InvertPathMode == false)
                        {
                            bool found = Fresul.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                            if(found)
                            {
                                foundMatch = true;
                                Server.ExecuteCommand($"exec Map-Configs-Prefix/{shortFileName}");
                                break;
                            }
                        }else if(Config.InvertPathMode == true)
                        {
                            bool found = Fmap.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                            if(found)
                            {
                                foundMatch = true;
                                Server.ExecuteCommand($"exec Map-Configs-Prefix/{shortFileName}");
                                break;
                            }
                        }
                        
                    }
                }
                if (!foundMatch)
                {
                    bool foundMatch2 = false;
                    foreach (string fileName in fileNamess)
                    {
                        
                        string shortFileName = Path.GetFileNameWithoutExtension(fileName);
                        if (!string.IsNullOrEmpty(shortFileName))
                        {
                            if(Config.InvertPathMode == false)
                            {
                                bool found = Fresul2.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                if(found)
                                {
                                    foundMatch2 = true;
                                    Server.ExecuteCommand($"exec Map-Configs-Prefix/{shortFileName}");
                                    break;
                                }
                            }else if(Config.InvertPathMode == true)
                            {
                                bool found = Fresul2.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                if(found)
                                {
                                    foundMatch2 = true;
                                    Server.ExecuteCommand($"exec Map-Configs-Prefix/{fileName}");
                                    break;
                                }
                            }
                            
                        }
                    }
                    if (!foundMatch2)
                    {
                        foreach (string fileName in fileNamess)
                        {
                            
                            string shortFileName = Path.GetFileNameWithoutExtension(fileName);
                            if (!string.IsNullOrEmpty(shortFileName))
                            {
                                if(Config.InvertPathMode == false)
                                {
                                    bool found = Fmap.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                    if(found)
                                    {
                                        Server.ExecuteCommand($"exec Map-Configs-Prefix/{shortFileName}");
                                        break;
                                    }
                                }else if(Config.InvertPathMode == true)
                                {
                                    bool found = Fresul.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                    if(found)
                                    {
                                        Server.ExecuteCommand($"exec Map-Configs-Prefix/{shortFileName}");
                                        break;
                                    }
                                }
                                
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