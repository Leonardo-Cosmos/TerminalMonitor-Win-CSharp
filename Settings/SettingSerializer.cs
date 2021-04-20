/* 2021/4/19 */
using System;
using System.IO;
using System.Text.Json;

namespace TerminalMonitor.Settings
{
    static class SettingSerializer
    {
        private const string settingFilePath = "./setting.json";

        public static void Save(TerminalMonitorSetting setting)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
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

        public static TerminalMonitorSetting Load()
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
