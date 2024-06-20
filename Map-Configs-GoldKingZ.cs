using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Core.Attributes;
using Map_Configs_GoldKingZ.Config;

namespace Map_Configs_GoldKingZ;

[MinimumApiVersion(234)]
public class MapConfigsGoldKingZ : BasePlugin
{
    public override string ModuleName => "Map Configs (Map Configs Depend Map Name)";
    public override string ModuleVersion => "1.0.6";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    
    public override void Load(bool hotReload)
    {
        Configs.Load(ModuleDirectory);
        Configs.Shared.CookiesModule = ModuleDirectory;
        Configs.Shared.CookiesGameDirectory = Server.GameDirectory;

        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterEventHandler<EventRoundAnnounceMatchStart>(OnMatchStart);
        RegisterEventHandler<EventRoundAnnounceWarmup>(OnWarmupStart);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
    }

    private void OnMapStart(string Map)
    {
        if(Configs.GetConfigData().RemoveMapCommands)
        {
            var pointservercommand = Utilities.FindAllEntitiesByDesignerName<CPointServerCommand>("point_servercommand");
            foreach (var ent in pointservercommand)
            {
                if (ent == null) continue;
                ent.Remove();
            }
        }

        if (Configs.GetConfigData().ExecMode.Contains("OnMapStart"))
        {
            int x = Configs.GetConfigData().ExecXTimes;
           Helper.ExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                       Helper.ExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }

        if (Configs.GetConfigData().ForceExecMode.Contains("OnMapStart"))
        {
            int x = Configs.GetConfigData().ForceExecXTimes;
           Helper. ForceExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                       Helper. ForceExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        

        string Fpath = Path.Combine(ModuleDirectory,"../../plugins/Map-Configs-GoldKingZ/ErrorLogs/");
        Globals.Date = DateTime.Now.ToString("MM-dd-yyyy");
        string fileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
        Globals.Tpath = Path.Combine(ModuleDirectory,"../../plugins/Map-Configs-GoldKingZ/ErrorLogs/") + $"{fileName}";

        if(Configs.GetConfigData().EnableErrorLogChecker && !Directory.Exists(Fpath))
        {
            Directory.CreateDirectory(Fpath);
        }

        if(Configs.GetConfigData().EnableErrorLogChecker && !File.Exists(Globals.Tpath))
        {
            using (File.Create(Globals.Tpath)) { }
        }
    }
    private HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo _)
    {
        if(@event == null)return HookResult.Continue;

        if (Configs.GetConfigData().ExecMode.Contains("OnPlayerSpawn"))
        {
            int x = Configs.GetConfigData().ExecXTimes;
           Helper.ExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                       Helper.ExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        if (Configs.GetConfigData().ForceExecMode.Contains("OnPlayerSpawn"))
        {
            int x = Configs.GetConfigData().ForceExecXTimes;
           Helper. ForceExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                       Helper. ForceExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }

        return HookResult.Continue;
    }

    private HookResult OnMatchStart(EventRoundAnnounceMatchStart @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        if (Configs.GetConfigData().ExecMode.Contains("OnMatchStart"))
        {
            int x = Configs.GetConfigData().ExecXTimes;
           Helper.ExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                       Helper.ExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        if (Configs.GetConfigData().ForceExecMode.Contains("OnMatchStart"))
        {
            int x = Configs.GetConfigData().ForceExecXTimes;
           Helper. ForceExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                       Helper. ForceExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        
        return HookResult.Continue;
    }
    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        if (Configs.GetConfigData().ExecMode.Contains("OnRoundStart"))
        {
            int x = Configs.GetConfigData().ExecXTimes;
           Helper.ExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                       Helper.ExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        if (Configs.GetConfigData().ForceExecMode.Contains("OnRoundStart"))
        {
            int x = Configs.GetConfigData().ForceExecXTimes;
           Helper. ForceExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                       Helper. ForceExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        
        return HookResult.Continue;
    }

    private HookResult OnWarmupStart(EventRoundAnnounceWarmup @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        if (Configs.GetConfigData().ExecMode.Contains("OnWarmupStart"))
        {
            int x = Configs.GetConfigData().ExecXTimes;
           Helper.ExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                       Helper.ExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        if (Configs.GetConfigData().ForceExecMode.Contains("OnWarmupStart"))
        {
            int x = Configs.GetConfigData().ForceExecXTimes;
           Helper. ForceExecCommandMap();
            Server.NextFrame(() =>
            {
                for (int i = 1; i <= x; i++)
                {
                    float interval = i * 1.0f;

                    AddTimer(interval, () =>
                    {
                       Helper. ForceExecCommandMap();
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            });
        }
        
        return HookResult.Continue;
    }

    
}