using System;
using System.IO;
using System.Text.Json;

namespace DrukEtykietAdv
{
    public class ConfigManager
    {
        public static Config LoadConfig(string configFilePath)
        {
            try
            {
                string json = File.ReadAllText(configFilePath);
                return JsonSerializer.Deserialize<Config>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load configuration: {ex.Message}");
                throw;
            }
        }
    }
}