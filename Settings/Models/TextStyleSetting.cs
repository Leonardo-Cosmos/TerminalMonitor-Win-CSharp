/* 2021/5/26 */
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    record TextStyleSetting(ColorSetting Foreground, ColorSetting Background);

    static class TextStyleSettings
    {
        public static TextStyleSetting Save(TextStyle obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new TextStyleSetting(
                Foreground: ColorSettings.Save(obj.Foreground),
                Background: ColorSettings.Save(obj.Background)
                );
        }

        public static TextStyle Load(TextStyleSetting setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new TextStyle()
            {
                Foreground = ColorSettings.Load(setting.Foreground),
                Background = ColorSettings.Load(setting.Background),
            };
        }
    }
}
