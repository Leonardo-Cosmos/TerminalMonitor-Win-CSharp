/* 2023/4/18 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    record ColumnHeaderStyleSetting(ColorSetting? Foreground, ColorSetting? Background, ColorSetting? CellBackground,
        string? HorizontalAlignment, string? VerticalAlignment, string? TextAlignment);

    static class ColumnHeaderStyleSettings
    {
        public static ColumnHeaderStyleSetting? Save(ColumnHeaderStyle? obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new ColumnHeaderStyleSetting(
                Foreground: ColorSettings.Save(obj.Foreground),
                Background: ColorSettings.Save(obj.Background),
                CellBackground: ColorSettings.Save(obj.CellBackground),
                HorizontalAlignment: HorizontalAlignmentSettings.Save(obj.HorizontalAlignment),
                VerticalAlignment: VerticalAlignmentSettings.Save(obj.VerticalAlignment),
                TextAlignment: TextAlignmentSettings.Save(obj.TextAlignment)
                );
        }

        public static ColumnHeaderStyle? Load(ColumnHeaderStyleSetting? setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new ColumnHeaderStyle()
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
