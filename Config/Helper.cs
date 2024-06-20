using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.RegularExpressions;
using Map_Configs_GoldKingZ.Config;

namespace Map_Configs_GoldKingZ;

public class Helper
{
    public static void AdvancedPrintToChat(CCSPlayerController player, string message, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                player.PrintToChat(" " + messages);
            }
        }else
        {
            player.PrintToChat(message);
        }
    }
    public static void AdvancedPrintToServer(string message, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                Server.PrintToChatAll(" " + messages);
            }
        }else
        {
            Server.PrintToChatAll(message);
        }
    }
    
    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        var excludedGroups = groups.Split(',');
        foreach (var group in excludedGroups)
        {
            if (group.StartsWith("#"))
            {
                if (AdminManager.PlayerInGroup(player, group))
                    return true;
            }
            else if (group.StartsWith("@"))
            {
                if (AdminManager.PlayerHasPermissions(player, group))
                    return true;
            }
        }
        return false;
    }
    public static List<CCSPlayerController> GetCounterTerroristController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.Team == CsTeam.CounterTerrorist).ToList();
        return playerList;
    }
    public static List<CCSPlayerController> GetTerroristController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.Team == CsTeam.Terrorist).ToList();
        return playerList;
    }
    public static List<CCSPlayerController> GetAllController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected).ToList();
        return playerList;
    }
    public static int GetCounterTerroristCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.TeamNum == (byte)CsTeam.CounterTerrorist);
    }
    public static int GetTerroristCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.TeamNum == (byte)CsTeam.Terrorist);
    }
    public static int GetAllCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected);
    }
    public static void ClearVariables()
    {
        
    }
    
    public static string ReplaceMessages(string Message, string Date, string time, string PlayerName, string SteamId, string ipAddress, string reason)
    {
        var replacedMessage = Message
                                    .Replace("{TIME}", time)
                                    .Replace("{Globals.Date}", Globals.Date)
                                    .Replace("{PLAYERNAME}", PlayerName.ToString())
                                    .Replace("{STEAMID}", SteamId.ToString())
                                    .Replace("{IP}", ipAddress.ToString())
                                    .Replace("{REASON}", reason);
        return replacedMessage;
    }
    public static string RemoveLeadingSpaces(string content)
    {
        string[] lines = content.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].TrimStart();
        }
        return string.Join("\n", lines);
    }
    private static CCSGameRules? GetGameRules()
    {
        try
        {
            var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            return gameRulesEntities.First().GameRules;
        }
        catch
        {
            return null;
        }
    }
    public static bool IsWarmup()
    {
        return GetGameRules()?.WarmupPeriod ?? false;
    }
    public static void ExecCommandMap()
    {
        string folderPath = Path.Combine(Configs.Shared.CookiesModule!, "../../plugins/Map-Configs-GoldKingZ");
        if(Globals.SMapName == null)return;
        int underscoreIndex = Globals.SMapName.IndexOf('_');
        int nextUnderscoreIndex = Globals.SMapName.IndexOf('_', underscoreIndex + 1);
        string prefix = underscoreIndex != -1 ? Globals.SMapName.Substring(0, underscoreIndex + 1) : Globals.SMapName;
        string prefix2 = underscoreIndex != -1 ? Globals.SMapName.Substring(0, nextUnderscoreIndex + 1) : Globals.SMapName;
        for (int i = 0; i < 4; i++)
        {
            folderPath = Path.Combine(folderPath, "..");
        }
        
        string cfgDirectory = Path.Combine(folderPath, "cfg");
        if (Directory.Exists(cfgDirectory))
        {
            string mapsCfgDirectory = Path.Combine(cfgDirectory, "Map-Configs");
            if (Directory.Exists(mapsCfgDirectory))
            {
                string[] fileNamess = Directory.GetFiles(mapsCfgDirectory);
                bool foundMatch = false;
                foreach (string fileName in fileNamess)
                {
                    
                    string shortFileName = Path.GetFileNameWithoutExtension(fileName);
                    
                    if (!string.IsNullOrEmpty(shortFileName))
                    {
                        if(Configs.GetConfigData().InvertPathMode == false)
                        {
                            bool found = prefix.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                            if(found)
                            {
                                foundMatch = true;
                                Server.ExecuteCommand($"exec Map-Configs/{shortFileName}");
                                break;
                            }
                        }else if(Configs.GetConfigData().InvertPathMode == true)
                        {
                            bool found = Globals.SMapName.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                            if(found)
                            {
                                foundMatch = true;
                                Server.ExecuteCommand($"exec Map-Configs/{shortFileName}");
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
                            if(Configs.GetConfigData().InvertPathMode == false)
                            {
                                bool found = prefix2.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                if(found)
                                {
                                    foundMatch2 = true;
                                    Server.ExecuteCommand($"exec Map-Configs/{shortFileName}");
                                    break;
                                }
                            }else if(Configs.GetConfigData().InvertPathMode == true)
                            {
                                bool found = prefix2.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                if(found)
                                {
                                    foundMatch2 = true;
                                    Server.ExecuteCommand($"exec Map-Configs/{fileName}");
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
                                if(Configs.GetConfigData().InvertPathMode == false)
                                {
                                    bool found = Globals.SMapName.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                    if(found)
                                    {
                                        foundMatch3 = true;
                                        Server.ExecuteCommand($"exec Map-Configs/{shortFileName}");
                                        break;
                                    }
                                }else if(Configs.GetConfigData().InvertPathMode == true)
                                {
                                    bool found = prefix.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                    if(found)
                                    {
                                        foundMatch3 = true;
                                        Server.ExecuteCommand($"exec Map-Configs/{shortFileName}");
                                        break;
                                    }
                                }
                                
                            }
                        }
                        if (!foundMatch3)
                        {
                            if (Configs.GetConfigData().EnableErrorLogChecker && File.Exists(Globals.Tpath))
                            {
                                try
                                {
                                    File.AppendAllLines(Globals.Tpath, new[]{$"[{Globals.Date} - Couldn't Find cfg in csgo/cfg/Map-Configs/ That Match Map Name {Globals.SMapName}]"});
                                }catch
                                {
                                    
                                }
                            }

                            string defaultFileName = "_default_.cfg";
                            string defaultFilePath = Path.Combine(mapsCfgDirectory, defaultFileName);
                            if (File.Exists(defaultFilePath))
                            {
                                Server.ExecuteCommand($"exec Map-Configs/{defaultFileName}");
                            }else if (!File.Exists(defaultFilePath))
                            {
                                if (Configs.GetConfigData().EnableErrorLogChecker && File.Exists(Globals.Tpath))
                                {
                                    try
                                    {
                                        File.AppendAllLines(Globals.Tpath, new[]{$"[{Globals.Date} - csgo/cfg/Map-Configs/_default_.cfg file does not exist.]"});
                                    }catch
                                    {
                                        
                                    }
                                }
                            }
                        }
                    }
                }  
            }
            else
            {
                if (Configs.GetConfigData().EnableErrorLogChecker && File.Exists(Globals.Tpath))
                {
                    try
                    {
                        File.AppendAllLines(Globals.Tpath, new[]{$"[{Globals.Date} - csgo/cfg/Map-Configs/ directory does not exist.]"});
                    }catch
                    {
                        
                    }
                }
                
            }
        }
        else
        {
            if (Configs.GetConfigData().EnableErrorLogChecker && File.Exists(Globals.Tpath))
            {
                try
                {
                    File.AppendAllLines(Globals.Tpath, new[]{$"[{Globals.Date} - csgo/cfg/ directory does not exist.]"});
                }catch
                {
                    
                }
            }
        }
    }
    public static void ForceExecCommandMap()
    {
        string folderPath = Path.Combine(Configs.Shared.CookiesModule!, "../../plugins/Map-Configs-GoldKingZ");
        if(Globals.SMapName == null)return;
        int underscoreIndex = Globals.SMapName.IndexOf('_');
        int nextUnderscoreIndex = Globals.SMapName.IndexOf('_', underscoreIndex + 1);
        string prefix = underscoreIndex != -1 ? Globals.SMapName.Substring(0, underscoreIndex + 1) : Globals.SMapName;
        string prefix2 = underscoreIndex != -1 ? Globals.SMapName.Substring(0, nextUnderscoreIndex + 1) : Globals.SMapName;

        string Fresul = "f_" + prefix;
        string Fresul2 = "f_" + prefix2;
        string Fmap = "f_" + Globals.SMapName;

        for (int i = 0; i < 4; i++)
        {
            folderPath = Path.Combine(folderPath, "..");
        }
        
        string cfgDirectory = Path.Combine(folderPath, "cfg");
        if (Directory.Exists(cfgDirectory))
        {
            string mapsCfgDirectory = Path.Combine(cfgDirectory, "Map-Configs");
            if (Directory.Exists(mapsCfgDirectory))
            {
                string[] fileNamess = Directory.GetFiles(mapsCfgDirectory);
                bool foundMatch = false;
                foreach (string fileName in fileNamess)
                {
                    
                    string shortFileName = Path.GetFileNameWithoutExtension(fileName);
                    
                    if (!string.IsNullOrEmpty(shortFileName))
                    {
                        if(Configs.GetConfigData().InvertPathMode == false)
                        {
                            bool found = Fresul.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                            if(found)
                            {
                                foundMatch = true;
                                Server.ExecuteCommand($"exec Map-Configs/{shortFileName}");
                                break;
                            }
                        }else if(Configs.GetConfigData().InvertPathMode == true)
                        {
                            bool found = Fmap.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                            if(found)
                            {
                                foundMatch = true;
                                Server.ExecuteCommand($"exec Map-Configs/{shortFileName}");
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
                            if(Configs.GetConfigData().InvertPathMode == false)
                            {
                                bool found = Fresul2.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                if(found)
                                {
                                    foundMatch2 = true;
                                    Server.ExecuteCommand($"exec Map-Configs/{shortFileName}");
                                    break;
                                }
                            }else if(Configs.GetConfigData().InvertPathMode == true)
                            {
                                bool found = Fresul2.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                if(found)
                                {
                                    foundMatch2 = true;
                                    Server.ExecuteCommand($"exec Map-Configs/{fileName}");
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
                                if(Configs.GetConfigData().InvertPathMode == false)
                                {
                                    bool found = Fmap.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                    if(found)
                                    {
                                        Server.ExecuteCommand($"exec Map-Configs/{shortFileName}");
                                        break;
                                    }
                                }else if(Configs.GetConfigData().InvertPathMode == true)
                                {
                                    bool found = Fresul.Trim().Equals(shortFileName.Trim(), StringComparison.OrdinalIgnoreCase);
                                    if(found)
                                    {
                                        Server.ExecuteCommand($"exec Map-Configs/{shortFileName}");
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
                if (Configs.GetConfigData().EnableErrorLogChecker && File.Exists(Globals.Tpath))
                {
                    try
                    {
                        File.AppendAllLines(Globals.Tpath, new[]{$"[{Globals.Date} - csgo/cfg/Map-Configs/ directory does not exist.]"});
                    }catch
                    {
                        
                    }
                }
                
            }
        }
        else
        {
            if (Configs.GetConfigData().EnableErrorLogChecker && File.Exists(Globals.Tpath))
            {
                try
                {
                    File.AppendAllLines(Globals.Tpath, new[]{$"[{Globals.Date} - csgo/cfg/ directory does not exist.]"});
                }catch
                {
                    
                }
            }
        }
    }
}