using System.Text.Json;
using System.Text.Json.Serialization;

namespace Map_Configs_GoldKingZ.Config
{
    public static class Configs
    {
        public static class Shared {
            public static string? CookiesModule { get; set; }
            public static string? CookiesGameDirectory { get; set; }
        }
        
        private static readonly string ConfigDirectoryName = "config";
        private static readonly string ConfigFileName = "config.json";
        private static string? _configFilePath;
        private static ConfigData? _configData;

        private static readonly JsonSerializerOptions SerializationOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter()
            },
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        public static bool IsLoaded()
        {
            return _configData is not null;
        }

        public static ConfigData GetConfigData()
        {
            if (_configData is null)
            {
                throw new Exception("Config not yet loaded.");
            }
            
            return _configData;
        }

        public static ConfigData Load(string modulePath)
        {
            var configFileDirectory = Path.Combine(modulePath, ConfigDirectoryName);
            if(!Directory.Exists(configFileDirectory))
            {
                Directory.CreateDirectory(configFileDirectory);
            }

            _configFilePath = Path.Combine(configFileDirectory, ConfigFileName);
            if (File.Exists(_configFilePath))
            {
                _configData = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(_configFilePath), SerializationOptions);
            }
            else
            {
                _configData = new ConfigData();
            }

            if (_configData is null)
            {
                throw new Exception("Failed to load configs.");
            }

            SaveConfigData(_configData);
            
            return _configData;
        }

        private static void SaveConfigData(ConfigData configData)
        {
            if (_configFilePath is null)
            {
                throw new Exception("Config not yet loaded.");
            }
            string json = JsonSerializer.Serialize(configData, SerializationOptions);


            File.WriteAllText(_configFilePath, json);
        }

        public class ConfigData
        {
            public bool RemoveMapCommands { get; set; }
            public bool InvertPathMode { get; set; }
            public string ExecMode { get; set; }
            public int ExecXTimes { get; set; }
            public string empty { get; set; }
            public string ForceExecMode  { get; set; }
            public int ForceExecXTimes { get; set; }
            public string empty2 { get; set; }
            public bool EnableErrorLogChecker  { get; set; }
            public string empty3 { get; set; }
            public string Information_For_You_Dont_Delete_it { get; set; }
            
            public ConfigData()
            {
                RemoveMapCommands = true;
                InvertPathMode = false;
                ExecMode = "OnRoundStart,OnMapStart,OnMatchStart,OnWarmupStart";
                ExecXTimes = 3;
                empty = "-----------------------------------------------------------------------------------";
                ForceExecMode = "OnPlayerSpawn";
                ForceExecXTimes = 3;
                empty2 = "-----------------------------------------------------------------------------------";
                EnableErrorLogChecker = false;
                empty3 = "-----------------------------------------------------------------------------------";
                Information_For_You_Dont_Delete_it = " Vist  [https://github.com/oqyh/cs2-Map-Configs-GoldKingZ/tree/main?tab=readme-ov-file#-configuration-] To Understand All Above";
            }
        }
    }
}