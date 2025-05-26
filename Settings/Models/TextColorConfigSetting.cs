/* 2022/5/8 */
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    record TextColorConfigSetting(string? Mode, ColorSetting? Color);

    static class TextColorConfigSettings
    {
        public static TextColorConfigSetting? Save(TextColorConfig? obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new TextColorConfigSetting(
                    Mode: TextColorModeSettings.Save(obj.Mode),
                    Color: ColorSettings.Save(obj.Color)
                );
        }

        public static TextColorConfig? Load(TextColorConfigSetting? setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new TextColorConfig()
            {
                Mode = TextColorModeSettings.Load(setting.Mode) ?? TextColorMode.Static,
                Color = ColorSettings.Load(setting.Color),
            };
        }
    }
}
