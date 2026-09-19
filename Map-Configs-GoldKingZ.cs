using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;

namespace Map_Configs_GoldKingZ;

public partial class MainPlugin : BasePlugin
{
    public override string ModuleName => "[Map Configs] Execute Configs And Load/Unload Plugins Per Map";
    public override string ModuleVersion => "1.0.7";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";

    public static MainPlugin Instance { get; private set; } = null!;

    public override void Load(bool hotReload)
    {
        Instance = this;

        Configs.Load(hotReload);

        Helper.RemoveRegisterCommandsAndHooks();
        Globals.Clear();
        Helper.RegisterCommandsAndHooks();
        Helper.ReloadPlayersGlobals();
    }

    public override void Unload(bool hotReload)
    {
        try
        {
            Helper.RemoveRegisterCommandsAndHooks();
            Globals.Clear();
        }
        catch (Exception ex)
        {
            Helper.Debug($"Unload Error: {ex.Message}", true);
        }
    }

    
    /* [ConsoleCommand("css_test", "test")]
    public void tesstttt(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(player == null || !player.IsValid)return;

    } */
}