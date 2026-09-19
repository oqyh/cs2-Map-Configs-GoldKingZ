using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.UserMessages;

namespace Map_Configs_GoldKingZ;

public static class Game_Commands
{
    public static HookResult HandlePlayerMessage(CCSPlayerController? player, string? rawMessage, UserMessage? um = null)
    {
        if (!player.IsValid() || string.IsNullOrWhiteSpace(rawMessage)) return HookResult.Continue;

        var message = rawMessage.Trim();

        if (Configs.Instance.Reload_Plugin.Reload_Plugin_CommandsInGame.ConvertCommands(true)?.Any(c => message.Equals(c.Trim(), StringComparison.OrdinalIgnoreCase)) == true)
        {
            Handle_ReloadPlugin(player, null!, um!);
        }

        return HookResult.Continue;
    }
    
    #region Handles

    public static void Handle_ReloadPlugin(CCSPlayerController player, CommandInfo commandInfo = null!, UserMessage um = null!)
    {
        if (!Globals.Player_Data.TryGetValue(player.Slot, out var playerData)) return;

        bool onetime = (DateTime.Now - playerData.EventPlayerChat).TotalSeconds > 0.4;
        if (onetime) playerData.EventPlayerChat = DateTime.Now;

        if (Configs.Instance.Reload_Plugin.Reload_Plugin_Flags.HasValidPermissionData() && !Helper.IsPlayerInGroupPermission(player, Configs.Instance.Reload_Plugin.Reload_Plugin_Flags))
        {
            if (onetime)
            {
                Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.ReloadPlugin.Not.Allowed"]);
            }
        }
        else
        {
            if (onetime)
            {
                Server.NextFrame(()=>
                {
                    Helper.RemoveRegisterCommandsAndHooks();
                    Globals.Clear();
                    Configs.Load(true);
                    Helper.RegisterCommandsAndHooks();
                    Helper.ReloadPlayersGlobals();
                });

                Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.ReloadPlugin.Successfully"]);
            }

            Helper.MuteCommands(um, Configs.Instance.Reload_Plugin.Reload_Plugin_Hide);
        }

        Helper.MuteCommands(um, Configs.Instance.Reload_Plugin.Reload_Plugin_Hide, true);
    }

    #endregion Handles
}
