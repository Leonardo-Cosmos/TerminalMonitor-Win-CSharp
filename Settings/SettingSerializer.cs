/* 2021/4/19 */
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TerminalMonitor.Settings
{
    static class SettingSerializer
    {
        private const string settingFilePath = "./setting.json";

        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public static void Save(TerminalMonitorSetting setting)
        {
            var json = JsonSerializer.Serialize<TerminalMonitorSetting>(setting, options);

            try
            {
                File.WriteAllText(settingFilePath, json);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.StackTrace);
            }
        }

        public static TerminalMonitorSetting? Load()
        {
            string json;
            try
            {
                json = File.ReadAllText(settingFilePath);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.StackTrace);
                return null;
            }

            var setting = JsonSerializer.Deserialize<TerminalMonitorSetting>(json);
            return setting;
        }
    }
}
