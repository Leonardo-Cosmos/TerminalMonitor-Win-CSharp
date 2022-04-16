/* 2021/5/26 */
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    record TextStyleSetting(ColorSetting Foreground, ColorSetting Background, ColorSetting CellBackground,
        string HorizontalAlignment, string VerticalAlignment, string TextAlignment);

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
                Background: ColorSettings.Save(obj.Background),
                CellBackground: ColorSettings.Save(obj.CellBackground),
                HorizontalAlignment: HorizontalAlignmentSettings.Save(obj.HorizontalAlignment),
                VerticalAlignment: VerticalAlignmentSettings.Save(obj.VerticalAlignment),
                TextAlignment: TextAlignmentSettings.Save(obj.TextAlignment)
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
                CellBackground = ColorSettings.Load(setting.CellBackground),
                HorizontalAlignment = HorizontalAlignmentSettings.Load(setting.HorizontalAlignment),
                VerticalAlignment = VerticalAlignmentSettings.Load(setting.VerticalAlignment),
                TextAlignment = TextAlignmentSettings.Load(setting.TextAlignment),
            };
        }
    }
}
