using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace Map_Configs_GoldKingZ;

public partial class MainPlugin
{
    public void Register_CommandListeners()
    {
        AddCommandListener("say", OnPlayerSay, HookMode.Post);
        AddCommandListener("say_team", OnPlayerSay_Team, HookMode.Post);

        Helper.RegisterCssCommands(Configs.Instance.Reload_Plugin.Reload_Plugin_CommandsInGame.ConvertCommands(), "Commands To Reload Map Configs Plugin", CommandsAction_ReloadPlugin);
    }

    public void Remove_CommandListeners()
    {
        RemoveCommandListener("say", OnPlayerSay, HookMode.Post);
        RemoveCommandListener("say_team", OnPlayerSay_Team, HookMode.Post);

        Helper.RemoveCssCommands(Configs.Instance.Reload_Plugin.Reload_Plugin_CommandsInGame.ConvertCommands(), CommandsAction_ReloadPlugin);
    }

    public HookResult OnPlayerSay(CCSPlayerController? player, CommandInfo info)
    {
        return Game_Commands.HandlePlayerMessage(player, info.ArgString.Trim('"'));
    }

    public HookResult OnPlayerSay_Team(CCSPlayerController? player, CommandInfo info)
    {
        return Game_Commands.HandlePlayerMessage(player, info.ArgString.Trim('"'));
    }

    public static void CommandsAction_ReloadPlugin(CCSPlayerController? player, CommandInfo info)
    {
        if (!player.IsValid()) return;

        Game_Commands.Handle_ReloadPlugin(player, info, null!);
    }
}
