# [CS2] Map-Configs-Prefix (1.0.3)

### Map Configs Depend Map Name

・Add Many Cfg You Like Depend Map Name to Execute Per Map Inside `csgo/cfg/Map-Configs-Prefix/`

Example:

・Prefix (`surf_.cfg`) will override any map start with surf_

・if fails plugin will start search to full map name (`surf_boreas.cfg`)

・if fails plugin will start `_default_.cfg`



## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

## .:[ Configuration ]:.
```json
{
   // Make Log For Debug The Error Located in ../addons/counterstrikesharp/plugins/Map_Configs_Prefix/ErrorLogs/
  "EnableErrorLogChecker": false,
  
  //-----------------------------------------------------------------------------------------
  
  "ConfigVersion": 1
}
```


## .:[ Change Log ]:.
```
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
