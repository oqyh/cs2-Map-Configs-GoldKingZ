using CounterStrikeSharp.API;

namespace Map_Configs_GoldKingZ;

public static class MapNameExtensions
{
    public const string CfgFolder  = "Map-Configs-GoldKingZ";
    public const string AllMapsCfg = "_allmaps_";
    public static string CfgDirectory => Path.Join(Server.GameDirectory, "csgo", "cfg", CfgFolder);

    public static List<string> GetNameCandidates(this string mapName)
    {
        var list = new List<string>();

        if (mapName.Length == 0) return list;

        list.Add(mapName);

        for (var i = mapName.LastIndexOf('_'); i > 0; i = mapName.LastIndexOf('_', i - 1))
        {
            var prefix = mapName[..(i + 1)];
            if (prefix != mapName) list.Add(prefix);
        }

        return list;
    }

    public static List<string> ResolveMapCfgPaths(this string mapName)
    {
        var dir   = CfgDirectory;
        var files = new List<string>();

        Helper.Debug($"Searching Cfg For \"{mapName}\" In ../csgo/cfg/{CfgFolder}/");

        var allMaps = Path.Join(dir, AllMapsCfg + ".cfg");
        if (File.Exists(allMaps))
        {
            Helper.Debug($"   [FOUND] {AllMapsCfg}.cfg");
            files.Add(allMaps);
        }
        else
        {
            Helper.Debug($"   [MISS]  {AllMapsCfg}.cfg");
        }

        foreach (var candidate in mapName.GetNameCandidates())
        {
            var path = Path.Join(dir, candidate + ".cfg");

            if (File.Exists(path))
            {
                Helper.Debug($"   [FOUND] {candidate}.cfg");
                files.Add(path);
                return files;
            }

            Helper.Debug($"   [MISS]  {candidate}.cfg");
        }

        if (files.Count == 0)
            Helper.Debug($"No Cfg Found For \"{mapName}\", Create One In: {dir}", true);
        else
            Helper.Debug($"No Map-Specific Cfg For \"{mapName}\", Using {AllMapsCfg}.cfg Only");

        return files;
    }
}