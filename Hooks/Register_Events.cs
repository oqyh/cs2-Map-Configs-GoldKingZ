using CounterStrikeSharp.API.Core;

namespace Map_Configs_GoldKingZ;

public partial class MainPlugin
{
    public void Register_Events()
    {
        RegisterEventHandler<EventPlayerDisconnect>(OnEventPlayerDisconnect);
    }

    public void Remove_Events()
    {
        DeregisterEventHandler<EventPlayerDisconnect>(OnEventPlayerDisconnect);
    }
    
    public HookResult OnEventPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        var player = @event.Userid;
        if (!player.IsValid()) return HookResult.Continue;

        if(Globals.Player_Data.ContainsKey(player.Slot))
        {
            Globals.Player_Data.Remove(player.Slot);
        }
        return HookResult.Continue;
    }
}
