namespace Map_Configs_GoldKingZ;

public class Reload_Plugin
{
    [Comment("Note: Console_Commands Can Be Execute Via Both Console And Chat By (! or css_)")]
    [Comment("Making Both Console_Commands And Chat_Commands Empty = Disable")]
    [String("Console_Commands", "Chat_Commands")]
    public string Reload_Plugin_CommandsInGame { get; set; } = "Console_Commands: css_reloadmapconfigs,css_reloadmapconfig,css_reloadmc | Chat_Commands: ";

    [Comment("If [Reload_Plugin_CommandsInGame] Pass, Is There Any Specified Restricted Flags, Groups, SteamIDs")]
    [Comment("Example:")]
    [Comment("\"SteamIDs: 76561198206086993,STEAM_0:1:507335558 | Flags: @css/root,@css/admin | Groups: #css/root,#css/admin\"")]
    [Comment("\"SteamIDs:  | Flags:  | Groups: \" = To Allow Everyone")]
    [String("SteamIDs", "Flags", "Groups")]
    public string Reload_Plugin_Flags { get; set; } = "SteamIDs: 76561198206086993,STEAM_0:1:507335558 | Flags: @css/root,@css/admin | Groups: #css/root,#css/admin";

    [Comment("If [Reload_Plugin_Flags] Pass, Hide Chat After Execute Reload_Plugin_CommandsInGame?:")]
    [Comment("0 = No")]
    [Comment("1 = Yes, But Only After Toggle Successfully")]
    [Comment("2 = Yes, Hide All The Time")]
    [Range(0, 2)]
    public int Reload_Plugin_Hide { get; set; } = 0;
}

public partial class Configs
{
    [BreakLine("----------------------------[ ↓ Plugin Info ↓ ]----------------------------{nextline}")]
    [Info("Version")]
    [Info("Github")]
    public object __InfoSection { get; set; } = null!;

    [BreakLine("----------------------------[ ↓ Main Config ↓ ]----------------------------{nextline}")]

    [Comment("Reload Map Configs Plugin")]
    public Reload_Plugin Reload_Plugin { get; set; } = new();

    [BreakLine("----------------------------[ ↓ Utilities ↓ ]----------------------------{nextline}")]

    [Comment("Auto Update Signatures (In ../plugins/Map-Configs-GoldKingZ/gamedata/gamedata.json)?")]
    [Comment("true = Yes")]
    [Comment("false = No")]
    public bool AutoUpdateSignatures { get; set; } = true;

    [Comment("Enable Debug Plugin In Server Console?")]
    [Comment("true = Yes")]
    [Comment("false = No")]
    public bool EnableDebug { get; set; } = false;
}
