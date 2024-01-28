# [CS2] Map-Configs-Prefix (1.0.4)

#### Map Configs Depend Map Name

・Add Many Cfg You Like Depend Map Name to Execute Per Map Inside `csgo/cfg/Map-Configs-Prefix/`


### Example Normal Cfg (Will be edit on json `ExecMode` and `ExecXTimes`):
 
 ・Prefix (`surf_.cfg`) will execute on any map start with `surf_`
 ・if plugin cannot find (`surf_.cfg`)  will start search to full map name config (`surf_boreas.cfg`)
 ・if plugin cannot find (`surf_boreas.cfg`)  will execute  (`_default_.cfg`)


### Example Force Cfg (Will be edit on json `ForceExecMode` and `ForceExecXTimes`):

 ・Prefix (`f_surf_.cfg`) will execute on any map start with `surf_`
 ・if plugin cannot find (`f_surf_.cfg`) will start search to full map name config (`f_surf_boreas.cfg`)
 ・if plugin cannot find (`f_surf_boreas.cfg`)  will execute  (`_default_.cfg`)



## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

## .:[ Configuration ]:.
```json
{
  //-----------Event Calls Path-----------//
  //-------------------------------------//
  //OnMapStart: Will Be Called On Map Start 1 Time Only
  //OnWarmupStart: Will Be Called On WarmUp Only
  //OnRoundStart: Will Be Called On Every Round Start Include WarmUp
  //OnMatchStart: Will Be Called On Match Started (Round 1) 1 Time Only
  //OnPlayerSpawn: Will Be Called On Every Player Spawn
  //-------------------------------------//
  
//-----------------------------------------------------------------------------------------

  //This On Normal Cfg example de_.cfg or de_dust2.cfg or _default_.cfg
  "ExecMode": "OnMapStart,OnWarmupStart,OnRoundStart,OnMatchStart",
  //How Many Time You Want ExecMode To Be Exec (More = Better To Override)
  "ExecXTimes": 3,
  
//-----------------------------------------------------------------------------------------

  //This On Force Cfg example f_de_.cfg or f_de_dust2.cfg
  "ForceExecMode": "OnPlayerSpawn",
  //How Many Time You Want ForceExecMode To Be Exec (More = Better To Override)
  "ForceExecXTimes": 1,
  
//-----------------------------------------------------------------------------------------

  "EnableErrorLogChecker": false,
  
//-----------------------------------------------------------------------------------------
  "ConfigVersion": 1
}
```


## .:[ Change Log ]:.
```
(1.0.4)
-Fix Some Bugs
-Rework Prefix Plugin
-Added "ExecMode"
-Added "ExecXTimes"
-Added "ForceExecMode"
-Added "ForceExecXTimes"

(1.0.3)
-Fix Some Bugs
-Fix Warmup Not Execute cfg

(1.0.2)
-Fix Some Bugs
-Remove "ConVarEnforcer"
-Now Cfg will Override Any Map

(1.0.1)
-Fix Some Bugs

(1.0.0)
-Initial Release
```

## .:[ Donation ]:.

If this project help you reduce time to develop, you can give me a cup of coffee :)

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/oQYh)
