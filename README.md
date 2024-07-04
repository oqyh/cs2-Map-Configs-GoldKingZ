## .:[ Join Our Discord For Support ]:.

<a href="https://discord.com/invite/U7AuQhu"><img src="https://discord.com/api/guilds/651838917687115806/widget.png?style=banner2"></a>

***
# [CS2] Map-Configs-GoldKingZ (1.0.6)

### Map Configs Depend Map Name

・Add Many Cfg You Like Depend Map Name to Execute Per Map Inside `csgo/cfg/Map-Configs/`





#### Example Normal Cfg (Will be edit on json `ExecMode` and `ExecXTimes`):
 
 >・Prefix (`aim_.cfg`) will execute on any map start with `aim_`
 >
 >・if plugin cannot find (`aim_.cfg`)  will start search config name (`aim_deagle_.cfg`)
 >
 >・if plugin cannot find (`aim_deagle_.cfg`)  will start search full map name config (`aim_deagle_lego.cfg`)
 >
 >・if plugin cannot find (`aim_deagle_lego.cfg`)  will execute  (`_default_.cfg`)




#### Example Force Cfg (Will be edit on json `ForceExecMode` and `ForceExecXTimes`):

 >・Prefix (`f_aim_.cfg`) will execute on any map start with `aim_`
 >
 >・if plugin cannot find (`f_aim_.cfg`)  will start search config name (`f_aim_deagle_.cfg`)
 >
 >・if plugin cannot find (`f_aim_deagle_.cfg`)  will start search full map name config (`f_aim_deagle_lego.cfg`)

 > [!NOTE]  
 > Note: If you like to invert the search path enable `InvertPathMode`



## .:[ Dependencies ]:.

[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)

## .:[ Configuration ]:.

> [!CAUTION]
> Config Located In ..\addons\counterstrikesharp\plugins\Map-Configs-GoldKingZ\config\config.json      
> 

```json
{
  //Remove Maps Custom Commands
  "RemoveMapCommands": true,

  //Plugin Find Route Configs In csgo/cfg/Map-Configs-Prefix/
  //----------------------------------------------------------------
  //false: aim_.cfg ==> aim_deagle_.cfg ==> aim_deagle_lego.cfg ==> _default_.cfg
  //true: aim_deagle_lego.cfg ==> aim_deagle_.cfg ==> aim_.cfg ==> _default_.cfg
  "InvertPathMode": false,
  
//-----------------------------------------------------------------------------------------

  //-----------Event Calls Path-----------//
  //-------------------------------------//
  //OnMapStart: Will Be Called On Map Start 1 Time Only
  //OnWarmupStart: Will Be Called On WarmUp Only
  //OnRoundStart: Will Be Called On Every Round Start Include WarmUp
  //OnMatchStart: Will Be Called On Match Started (Round 1) 1 Time Only
  //OnPlayerSpawn: Will Be Called On Every Player Spawn
  //-------------------------------------//
  
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

  //Enable Error Debug Logs Located In ..\addons\counterstrikesharp\plugins\Map-Configs-GoldKingZ\ErrorLogs\
  "EnableErrorLogChecker": false,
}
```


## .:[ Change Log ]:.
```
(1.0.6)
-Rework Prefix Plugin
-Fix Some Bugs
-Fix EnableErrorLogChecker
-Added RemoveMapCommands

(1.0.5)
-Fix Some Bugs
-Rework Prefix Plugin
-Added InvertPathMode
-Added More prefix

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
