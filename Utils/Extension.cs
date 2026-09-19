using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;

namespace Map_Configs_GoldKingZ;

public static class Extension
{
    public static bool IsValid([NotNullWhen(true)] this CCSPlayerController? player, bool IncludeBots = false, bool IncludeHLTV = false)
    {
        if (player == null || !player.IsValid)
            return false;

        if (!IncludeBots && player.IsBot)
            return false;

        if (!IncludeHLTV && player.IsHLTV)
            return false;

        return true;
    }

    public static bool HasValidPermissionData(this string? groups)
    {
        if (string.IsNullOrWhiteSpace(groups)) return false;

        var segments = groups.Split('|', StringSplitOptions.RemoveEmptyEntries);
        foreach (var seg in segments)
        {
            var trimmed = seg.Trim();
            if (string.IsNullOrEmpty(trimmed))
                continue;

            int colonIndex = trimmed.IndexOf(':');
            if (colonIndex == -1 || colonIndex == 0)
                continue;

            string values = trimmed.Substring(colonIndex + 1).Trim();
            if (!string.IsNullOrEmpty(values))
                return true;
        }

        return false;
    }

    private const ulong Steam64Offset = 76561197960265728UL;
    public static (string steam2, string steam3, string steam32, string steam64) GetPlayerSteamID(this ulong steamId64)
    {
        uint id32 = (uint)(steamId64 - Steam64Offset);
        var steam32 = id32.ToString();
        uint y = id32 & 1;
        uint z = id32 >> 1;
        var steam2 = $"STEAM_0:{y}:{z}";
        var steam3 = $"[U:1:{steam32}]";
        var steam64 = steamId64.ToString();
        return (steam2, steam3, steam32, steam64);
    }

    public static string[]? ConvertCommands(this string input, bool EventPlayerChat = false)
    {
        var parts = input.Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Split(':', 2))
            .ToDictionary(
                p => p[0].Trim(),
                p => p.Length > 1
                    ? p[1].Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(c => c.Trim())
                        .Where(c => !string.IsNullOrEmpty(c))
                    : Enumerable.Empty<string>()
            );

        if (!parts.Values.Any(v => v.Any())) return null;

        if (!EventPlayerChat)
        {
            return parts.FirstOrDefault().Value?.Select(c =>
            {
                if (c.StartsWith("!"))
                {
                    var cmd = c.TrimStart('!');
                    return cmd.StartsWith("css_") ? cmd : "css_" + cmd;
                }
                return c;
            }).Distinct().ToArray();
        }

        var first = parts.FirstOrDefault().Value?
            .Select(c =>
            {
                var cmd = c.TrimStart('!');
                if (cmd.StartsWith("css_"))
                    cmd = cmd.Substring(4);
                return "!" + cmd;
            }) ?? Enumerable.Empty<string>();

        var rest = parts.Skip(1).SelectMany(p => p.Value);
        var result = first.Concat(rest).Distinct().ToArray();

        return result.Length == 0 ? null : result;
    }
}
