using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace VpnClientApp
{
    public class VpnConfig
    {
        public string VpnName { get; set; } = "MyVPN";
        public string ServerAddress { get; set; } = "";
        public string ConnectionType { get; set; } = "L2TP/IPSec";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public bool SaveCredentials { get; set; } = true;

        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VpnClientApp",
            "config.json");

        public static VpnConfig Load()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    return JsonSerializer.Deserialize<VpnConfig>(json) ?? new VpnConfig();
                }
            }
            catch (Exception)
            {
            }

            return new VpnConfig();
        }

        public void Save()
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(ConfigFilePath);
                if (!Directory.Exists(directoryPath) && directoryPath != null)
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception)
            {
            }
        }
    }
}
