using System.Text;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.UserMessages;

namespace Map_Configs_GoldKingZ;

public partial class MainPlugin
{
    public void Register_UserMessages()
    {
        HookUserMessage(118, OnUserMessage_OnSayText2, HookMode.Pre);
    }

    public void Remove_UserMessages()
    {
        UnhookUserMessage(118, OnUserMessage_OnSayText2, HookMode.Pre);
    }

    public HookResult OnUserMessage_OnSayText2(UserMessage um)
    {
        var player = Utilities.GetPlayerFromIndex(um.ReadInt("entityindex"));

        return Game_Commands.HandlePlayerMessage(player, Encoding.UTF8.GetString(um.ReadBytes("param2")), um);
    }
}