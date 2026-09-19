using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Map_Configs_GoldKingZ;

public partial class MainPlugin
{
    public void Register_Listeners()
    {
        RegisterListener<Listeners.OnClientConnected>(OnClientConnected);
        RegisterListener<Listeners.OnEntitySpawned>(OnEntitySpawned);
        RegisterListener<Listeners.OnEntityCreated>(OnEntityCreated);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
    }

    public void Remove_Listeners()
    {
        RemoveListener<Listeners.OnClientConnected>(OnClientConnected);
        RemoveListener<Listeners.OnEntitySpawned>(OnEntitySpawned);
        RemoveListener<Listeners.OnEntityCreated>(OnEntityCreated);
        RemoveListener<Listeners.OnMapEnd>(OnMapEnd);
    }
    
    public void OnClientConnected(int playerSlot)
    {
        var player = Utilities.GetPlayerFromSlot(playerSlot);
        if (player == null || !player.IsValid) return;

        Helper.CheckPlayerInGlobals(player);
    }

    public void OnEntitySpawned(CEntityInstance entity)
    {
        if (entity == null || !entity.IsValid) return;
        var designerName = entity.DesignerName;

        if (string.IsNullOrWhiteSpace(designerName) || !Helper.ShouldRemove(designerName)) return;
        entity.Remove();
        Helper.Debug($"[OnEntitySpawned] Removed ==> {designerName}");
        Server.NextFrame(()=>
        {
            if (entity == null || !entity.IsValid) return;
            entity.Remove();
        });
    }
    public void OnEntityCreated(CEntityInstance entity)
    {
        if (entity == null || !entity.IsValid) return;
        var designerName = entity.DesignerName;

        if (string.IsNullOrWhiteSpace(designerName) || !Helper.ShouldRemove(designerName)) return;
        entity.Remove();
        Helper.Debug($"[OnEntityCreated] Removed ==> {designerName}");
        Server.NextFrame(()=>
        {
            if (entity == null || !entity.IsValid) return;
            entity.Remove();
        });
    }

    public static void RemoveExistingCommands(string designerName)
    {
        if(designerName.Equals("point_servercommand"))
        {
            foreach (var ent in Utilities.FindAllEntitiesByDesignerName<CPointServerCommand>("point_servercommand"))
            {
                if (ent == null || !ent.IsValid) continue;
                Helper.Debug($"Removed ==> {designerName}");
                ent.Remove();
            }
        }else if(designerName.Equals("point_clientcommand"))
        {
            foreach (var ent in Utilities.FindAllEntitiesByDesignerName<CPointClientCommand>("point_clientcommand"))
            {
                if (ent == null || !ent.IsValid) continue;
                Helper.Debug($"Removed ==> {designerName}");
                ent.Remove();
            }
        }
    }
    
    public void OnMapEnd()
    {
        try
        {
            Globals.Clear();
        }
        catch (Exception ex)
        {
            Helper.Debug($"OnMapEnd Error: {ex.Message}", true);
        }
    }
}
