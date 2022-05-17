/* 2021/5/26 */
using System.Windows.Media;
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    record TextStyleSetting(ColorSetting Foreground, ColorSetting Background, ColorSetting CellBackground,
        TextColorConfigSetting ForegroundConfig, TextColorConfigSetting BackgroundConfig, TextColorConfigSetting CellBackgroundConfig,
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
                Foreground: null,
                Background: null,
                CellBackground: null,
                ForegroundConfig: TextColorConfigSettings.Save(obj.Foreground),
                BackgroundConfig: TextColorConfigSettings.Save(obj.Background),
                CellBackgroundConfig: TextColorConfigSettings.Save(obj.CellBackground),
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

            static TextColorConfig LoadColorConfig(TextColorConfig colorConfigSetting, Color? colorSetting)
            {
                if (colorConfigSetting != null)
                {
                    return colorConfigSetting;
                }

                if (colorSetting != null)
                {
                    return new TextColorConfig
                    {
                        Mode = TextColorMode.Static,
                        Color = colorSetting,
                    };
                }

                return null;
            }

            var foreground = LoadColorConfig(TextColorConfigSettings.Load(setting.ForegroundConfig),
                ColorSettings.Load(setting.Foreground));

            var background = LoadColorConfig(TextColorConfigSettings.Load(setting.BackgroundConfig),
                ColorSettings.Load(setting.Background));

            var cellBackground = LoadColorConfig(TextColorConfigSettings.Load(setting.CellBackgroundConfig),
                ColorSettings.Load(setting.CellBackground));

            return new TextStyle()
            {
                Foreground = foreground,
                Background = background,
                CellBackground = cellBackground,
                HorizontalAlignment = HorizontalAlignmentSettings.Load(setting.HorizontalAlignment),
                VerticalAlignment = VerticalAlignmentSettings.Load(setting.VerticalAlignment),
                TextAlignment = TextAlignmentSettings.Load(setting.TextAlignment),
            };
        }
    }
}
