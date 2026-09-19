using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace Map_Configs_GoldKingZ;

public partial class MainPlugin
{
    public void Register_CustomHooks()
    {
        CustomGameData.HookOffset<VirtualFunctionVoid<IntPtr, IntPtr, IntPtr, string>>("StartupServer", ValveInterface.NetworkServerService.Pointer, OnStartupServer, HookMode.Pre);
        CustomGameData.HookSignature<MemoryFunctionWithReturn<IntPtr, int, IntPtr, IntPtr, IntPtr, byte>>("OnConVarChanged", OnConVarChanged, HookMode.Pre);
    }

    public void Remove_CustomHooks()
    {
        CustomGameData.UnhookAll();
    }
    
    private HookResult OnStartupServer(DynamicHook hook)
    {
        try
        {
            var mapName = hook.GetParam<string>(3);
            if (string.IsNullOrWhiteSpace(mapName)) return HookResult.Continue;

            Helper.Debug($"Applying Map Settings For \"{mapName}\"");
            Helper.ApplyMapCfg(mapName);
            Helper.RestartOnGameModeAndTypeChanged();
            Globals.GetInfo();
        }
        catch (Exception ex)
        {
            Helper.Debug($"[OnStartupServer] Error: {ex.Message}", true);
        }

        return HookResult.Continue;
    }

    public static HookResult OnConVarChanged(DynamicHook hook)
    {
        try
        {
            var cvarRefPtr = hook.GetParam<IntPtr>(0);
            var slot       = hook.GetParam<int>(1);
            var valuePtr   = hook.GetParam<IntPtr>(2);

            if (cvarRefPtr == IntPtr.Zero || valuePtr == IntPtr.Zero) return HookResult.Continue;

            string cvarName = ReadConVarName(cvarRefPtr);
            if (string.IsNullOrEmpty(cvarName)) return HookResult.Continue;

            string newValue = Marshal.PtrToStringAnsi(valuePtr) ?? "";

            if (Globals.HookConVars.TryGetValue(cvarName, out string? expectedValue))
            {
                if (!newValue.Equals(expectedValue, StringComparison.OrdinalIgnoreCase))
                {
                    Helper.Debug($"[OnConVarChanged] Slot {slot} attempted to change \"{cvarName}\" from \"{expectedValue}\" to \"{newValue}\", forcing back to \"{expectedValue}\"");
                    
                    Server.ExecuteCommand($"{cvarName} {expectedValue}");
                    Server.NextFrame(() =>
                    {
                        Server.ExecuteCommand($"{cvarName} {expectedValue}");
                    });
                }
            }

            return HookResult.Continue;
        }
        catch (Exception ex)
        {
            Helper.Debug($"[OnConVarChanged] Error: {ex.Message}", true);
            return HookResult.Continue;
        }
    }

    private static string ReadConVarName(IntPtr cvarRefPtr)
    {
        try
        {
            var cvarPtr = Marshal.ReadIntPtr(cvarRefPtr, 0x8);
            if (cvarPtr == IntPtr.Zero) return "";

            var namePtr = Marshal.ReadIntPtr(cvarPtr, 0x0);
            if (namePtr == IntPtr.Zero) return "";

            return Marshal.PtrToStringAnsi(namePtr) ?? "";
        }
        catch
        {
            return "";
        }
    }
}