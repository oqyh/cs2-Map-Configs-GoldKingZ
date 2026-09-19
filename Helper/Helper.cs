using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Utils;


namespace Map_Configs_GoldKingZ;

public partial class Helper
{
    public static void RegisterCommandsAndHooks()
    {
        MainPlugin.Instance.Register_Listeners();
        MainPlugin.Instance.Register_Events();
        MainPlugin.Instance.Register_CommandListeners();
        MainPlugin.Instance.Register_UserMessages();
        MainPlugin.Instance.Register_CustomHooks();
        MainPlugin.Instance.Register_FakeConVar();
    }

    public static void RemoveRegisterCommandsAndHooks()
    {
        MainPlugin.Instance.Remove_Listeners();
        MainPlugin.Instance.Remove_Events();
        MainPlugin.Instance.Remove_CommandListeners();
        MainPlugin.Instance.Remove_UserMessages();
        MainPlugin.Instance.Remove_CustomHooks();
        MainPlugin.Instance.Remove_FakeConVar();
    }

    public static void RegisterCssCommands(string[]? commands, string description, CommandInfo.CommandCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrWhiteSpace(cmd)) continue;
            MainPlugin.Instance.AddCommand(cmd, description, callback);
        }
    }

    public static void RemoveCssCommands(string[]? commands, CommandInfo.CommandCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrWhiteSpace(cmd)) continue;
            MainPlugin.Instance.RemoveCommand(cmd, callback);
        }
    }

    public static void RegisterCssListener(string[]? commands, CommandInfo.CommandListenerCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrWhiteSpace(cmd)) continue;
            MainPlugin.Instance.AddCommandListener(cmd, callback, HookMode.Pre);
        }
    }

    public static void RemoveCssListener(string[]? commands, CommandInfo.CommandListenerCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrWhiteSpace(cmd)) continue;
            MainPlugin.Instance.RemoveCommandListener(cmd, callback, HookMode.Pre);
        }
    }

    public static List<CCSPlayerController> GetPlayersController(bool IncludeBots = false, bool IncludeHLTV = false, bool IncludeNone = true, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        try
        {
            return Utilities.GetPlayers()
                .Where(p =>
                    (IncludeBots || !p.IsBot) &&
                    (IncludeHLTV || !p.IsHLTV) &&
                    ((IncludeCT   && p.TeamNum == (byte)CsTeam.CounterTerrorist) ||
                    (IncludeT    && p.TeamNum == (byte)CsTeam.Terrorist) ||
                    (IncludeNone && p.TeamNum == (byte)CsTeam.None) ||
                    (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator)))
                .ToList();
        }
        catch (NativeException)
        {
            return new();
        }
    }

    public static void ReloadPlayersGlobals()
    {
        foreach (var players in GetPlayersController())
        {
            if(!players.IsValid()) continue;

            CheckPlayerInGlobals(players);
        }
    }

    public static void CheckPlayerInGlobals(CCSPlayerController player)
    {
        if(!player.IsValid()) return;

        if (!Globals.Player_Data.ContainsKey(player.Slot))
        {
            var initialData = new Globals.PlayerDataClass
            {
                Player = player,
            };
            Globals.Player_Data.TryAdd(player.Slot, initialData);
        }else
        {
            Globals.Player_Data[player.Slot].Player = player;
        }
    }

    public static void AdvancedPlayerPrintToChat(CCSPlayerController player, CommandInfo commandInfo, string message, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i]?.ToString() ?? "");
        }

        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrWhiteSpace(trimmedPart))
                {
                    if (commandInfo != null && commandInfo.CallingContext == CommandCallingContext.Console)
                    {
                        player.PrintToConsole(" " + trimmedPart);
                    }
                    else
                    {
                        player.PrintToChat(" " + trimmedPart);
                    }
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            if (commandInfo != null && commandInfo.CallingContext == CommandCallingContext.Console)
            {
                player.PrintToConsole(message);
            }
            else
            {
                player.PrintToChat(message);
            }
        }
    }

    public static void AdvancedServerPrintToChatAll(string message, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString() ?? "");
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrWhiteSpace(trimmedPart))
                {
                    Server.PrintToChatAll(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            Server.PrintToChatAll(message);
        }
    }

    public static void AdvancedPlayerPrintToConsole(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString() ?? "");
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrWhiteSpace(trimmedPart))
                {
                    player.PrintToConsole(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            player.PrintToConsole(message);
        }
    }

    public static void MuteCommands(UserMessage? um, int HideConfig, bool Fully = false)
    {
        if (um == null) return;
        if ((!Fully && HideConfig > 0) || (Fully && HideConfig == 2))
        {
            um.Recipients.Clear();
        }
    }

    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        if (string.IsNullOrWhiteSpace(groups) || player == null || !player.IsValid)
            return false;

        return groups.Split('|')
            .Select(segment => segment.Trim())
            .Any(trimmedSegment => Permission_CheckPermissionSegment(player, trimmedSegment));
    }

    private static bool Permission_CheckPermissionSegment(CCSPlayerController player, string segment)
    {
        if (string.IsNullOrWhiteSpace(segment)) return false;

        int colonIndex = segment.IndexOf(':');
        if (colonIndex == -1 || colonIndex == 0) return false;

        string prefix = segment.Substring(0, colonIndex).Trim().ToLower();
        string values = segment.Substring(colonIndex + 1).Trim();

        return prefix switch
        {
            "steamid" or "steamids" or "steam" or "steams" => Permission_CheckSteamIds(player, values),
            "flag" or "flags" => Permission_CheckFlags(player, values),
            "group" or "groups" => Permission_CheckGroups(player, values),
            _ => false
        };
    }

    private static bool Permission_CheckSteamIds(CCSPlayerController player, string steamIds)
    {
        if (string.IsNullOrWhiteSpace(steamIds)) return false;

        steamIds = steamIds.Replace("[", "").Replace("]", "");

        var (steam2, steam3, steam32, steam64) = player.SteamID.GetPlayerSteamID();
        var steam3NoBrackets = steam3.Trim('[', ']');

        return steamIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => id.Trim())
            .Any(trimmedId =>
                string.Equals(trimmedId, steam2, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam3NoBrackets, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam32, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam64, StringComparison.OrdinalIgnoreCase)
            );
    }

    private static bool Permission_CheckFlags(CCSPlayerController player, string flags)
    {
        if (player == null || !player.IsValid ||
            player.Connected != PlayerConnectedState.Connected ||
            player.IsBot || player.IsHLTV)
            return false;

        if (string.IsNullOrWhiteSpace(flags))
            return false;

        var playerData = AdminManager.GetPlayerAdminData(player);
        if (playerData == null)
            return false;

        var requiredFlags = flags
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToList();

        if (playerData._flags != null &&
            requiredFlags.Any(reqFlag =>
                playerData._flags.Contains(reqFlag, StringComparer.OrdinalIgnoreCase)))
            return true;

        var allFlags = playerData.GetAllFlags();
        return allFlags != null &&
            requiredFlags.Any(reqFlag =>
                allFlags.Contains(reqFlag, StringComparer.OrdinalIgnoreCase));
    }

    private static bool Permission_CheckGroups(CCSPlayerController player, string groups)
    {
        if (player == null || !player.IsValid ||
            player.Connected != PlayerConnectedState.Connected ||
            player.IsBot || player.IsHLTV)
            return false;

        if (string.IsNullOrWhiteSpace(groups))
            return false;

        var playerData = AdminManager.GetPlayerAdminData(player);
        if (playerData == null || playerData.Groups == null || !playerData.Groups.Any())
            return false;

        return groups
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(g => g.Trim())
            .Any(reqGroup => playerData.Groups.Contains(reqGroup, StringComparer.OrdinalIgnoreCase));
    }

    

    private const string Prefix = "[Map Configs]";
    public static void Debug(string message, bool important = false, Con? prefixColor = null)
    {
        if (!Configs.Instance.EnableDebug && !important) return;

        Con defaultColor = important ? Con.Red : Con.Magenta;
        prefixColor ??= Con.Purple;

        Con.WriteLine($"{prefixColor}{Prefix}: {defaultColor}{message}{Con.Reset}");
    }
}
