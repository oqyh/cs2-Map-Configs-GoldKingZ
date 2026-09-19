---

<h2 align="center">.:[ Community | Support ]:.</h2>
<p align="center">
  <a href="https://discord.com/invite/U7AuQhu">
    <img src="https://img.shields.io/badge/Discord-Join-5865F2?style=for-the-badge&logo=discord&logoColor=white" />
  </a>
  <a href="https://ko-fi.com/goldkingz">
    <img src="https://img.shields.io/badge/Ko--fi-Support-FF5E5B?style=for-the-badge&logo=kofi&logoColor=white" />
  </a>
  <a href="https://paypal.me/oQYh">
    <img src="https://img.shields.io/badge/PayPal-Donate-00457C?style=for-the-badge&logo=paypal&logoColor=white" />
  </a>
</p>

---

# [CS2] Map-Configs-GoldKingZ (1.0.7)

### Execute Configs And Load/Unload Plugins Per Map

<img width="900" height="1275" alt="Map-Configs-GoldKingZ 1 0 7" src="https://github.com/user-attachments/assets/24427a52-93ee-4a32-9df9-75f6e2502c3c" />


<details>
<summary><b>🖼️ Map Configs Previews</b> (Click to expand 🔽)</summary>
<br>


<p align="center">
  <a href="https://github.com/user-attachments/assets/4759bfe9-f3a8-4cef-b520-316a0196bbcc">
    <img src="https://github.com/user-attachments/assets/4759bfe9-f3a8-4cef-b520-316a0196bbcc" alt="image1" width="900">
  </a>
</p>

<p align="center">
  <a href="https://github.com/user-attachments/assets/e5055895-096c-4059-a43b-8ba007c04bc7">
    <img src="https://github.com/user-attachments/assets/e5055895-096c-4059-a43b-8ba007c04bc7" alt="image2" width="830">
  </a>
</p>

<p align="center">
  <a href="https://github.com/user-attachments/assets/bb9c7578-81c8-4b98-8625-8dd147207b70">
    <img src="https://github.com/user-attachments/assets/bb9c7578-81c8-4b98-8625-8dd147207b70" alt="image3" width="830">
  </a>
</p>

</details>


---

## 📦 Dependencies

[![Metamod:Source](https://img.shields.io/badge/Metamod:Source-REQUIRED_TO_DOWNLOAD-red?logo=sourceengine&labelColor=2d2d2d)](https://www.sourcemm.net)

[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-REQUIRED_TO_DOWNLOAD-red?logo=github&labelColor=83358F)](https://github.com/roflmuffin/CounterStrikeSharp)

[![JSON](https://img.shields.io/badge/JSON-INCLUDED_IN_ZIP-brightgreen?logo=json&labelColor=000000)](https://www.newtonsoft.com/json)

---

## 📥 Installation

### Plugin Installation
1. Download the latest `Map-Configs-GoldKingZ.x.x.x.zip` release
2. Extract contents to your `csgo` directory
3. Configure settings in `Map-Configs-GoldKingZ/config/config.json`
4. Restart your server

---

## ⚙️ Configuration
 
> [!IMPORTANT]
> **Main Configuration**  
> `../Map-Configs-GoldKingZ/config/config.json`  
> **Map Cfg Files**  
> `../csgo/cfg/Map-Configs-GoldKingZ/`

## 🛠️ `config/config.json`
<details open>
<summary><b>Main Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |
|----------|-------------|--------|----------|
| `Reload_Plugin_CommandsInGame` | Commands to reload the plugin (console/chat by `!` or `css_`) | `Console_Commands:` `Chat_Commands:`<br>Both empty = Disable | - |
| `Reload_Plugin_Flags` | Restrict reload command to SteamIDs, Flags, Groups | `SteamIDs:` `Flags:` `Groups:`<br>All empty = Allow everyone | `Reload_Plugin_CommandsInGame` |
| `Reload_Plugin_Hide` | Hide chat after executing reload command | `0`-No<br>`1`-Only after successful toggle<br>`2`-Hide all the time | `Reload_Plugin_Flags` |

</details>
<details>
<summary><b>Utilities Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |
|----------|-------------|--------|----------|
| `AutoUpdateSignatures` | Auto update signatures in `../Map-Configs-GoldKingZ/gamedata/gamedata.json` | `true`/`false` | - |
| `EnableDebug` | Enable debug in server console (helps debug issues) | `true`/`false` | - |

</details>

## 📁 `csgo/cfg/Map-Configs-GoldKingZ/`
<details open>
<summary><b>Lookup Order</b> (Click to expand 🔽)</summary>

Example for map `de_dust2_test`

| File | When It Executes |
|------|------------------|
| `_allmaps_.cfg` | If found, always execute first, on every map |
| `de_dust2_test.cfg` | If found, execute it and stop |
| `de_dust2_.cfg` | If not found above, try this |
| `de_.cfg` | If not found above, try this |
| (none) | Only `_allmaps_.cfg` is applied (if found) |

</details>

<details open>
<summary><b>Custom ConVars</b> (Click to expand 🔽)</summary>

Usable in any cfg above, or from server console

| ConVar | Description | Values |
|--------|-------------|--------|
| `gkz_load` | Load a plugin (also checks `plugins/disabled/`) | `gkz_load PluginName` |
| `gkz_unload` | Unload a plugin | `gkz_unload PluginName` |
| `gkz_unloadall` | Unload every plugin, except this one and the `gkz_load` lines in the same cfg | `true`/`false` |
| `gkz_removemapservercommands` | Remove map `point_servercommand` entities | `true`/`false` |
| `gkz_removemapclientcommands` | Remove map `point_clientcommand` entities | `true`/`false` |
| `gkz_forcerestart` | Force restart the map once after it loads | `true`/`false` |
| `gkz_exec` | Execute another cfg from `../csgo/cfg/` | `gkz_exec "gamemode_casual_server.cfg"`<br>`gkz_exec "myfolder/config.cfg"` |

</details>

> [!NOTE]
> Any ConVar works in these cfgs (cheat/hidden included).  
> ConVars set in the cfg are locked, no one can change them until map change or plugin reload.  
> Changing `game_mode` / `game_type` in the cfg will auto restart the map.

---

## 📜 Changelog

<details>
<summary><b>📋 View Version History</b> (Click to expand 🔽)</summary>

### [1.0.7]
- Upgrade Net.10
- CleanUp + Optimization
- Rework plugin
- Removed `_default_.cfg`
- Removed RemoveMapCommands To Prevent Crash On Some Maps
- Removed InvertPathMode
- Removed ExecMode
- Removed ExecXTimes
- Removed ForceExecMode
- Removed ForceExecXTimes
- Change EnableErrorLogChecker To EnableDebug
- Added AutoUpdateSignatures
- Added ConVar `gkz_load`
- Added ConVar `gkz_unload`
- Added ConVar `gkz_unloadall`
- Added ConVar `gkz_removemapservercommands`
- Added ConVar `gkz_removemapclientcommands`
- Added ConVar `gkz_forcerestart`
- Added ConVar `gkz_exec`
- Added `_allmaps_.cfg` To Apply All Maps
- Added StartupServer To SetUp Map Config Before `OnMapStart`
- Added OnConVarChanged To ConVar Lock
- Added Auto Restart Map When `game_mode` / `game_type` Changed

### [1.0.6]
- Rework prefix plugin
- Fix some bugs
- Fix `EnableErrorLogChecker`
- Added `RemoveMapCommands`

### [1.0.5]
- Fix some bugs
- Rework prefix plugin
- Added `InvertPathMode`
- Added more prefix

### [1.0.4]
- Fix some bugs
- Rework prefix plugin
- Added `ExecMode`
- Added `ExecXTimes`
- Added `ForceExecMode`
- Added `ForceExecXTimes`

### [1.0.3]
- Fix some bugs
- Fix warmup not executing cfg

### [1.0.2]
- Fix some bugs
- Removed `ConVarEnforcer`
- Now cfg will override any map

### [1.0.1]
- Fix some bugs

### [1.0.0]
- Initial plugin release

</details>

---
