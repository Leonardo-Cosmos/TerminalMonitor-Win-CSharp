/* 2021/5/26 */
using System.Windows.Media;

namespace TerminalMonitor.Settings.Models
{
    record ColorSetting(byte A, byte R, byte G, byte B);

    static class ColorSettings
    {
        public static ColorSetting? Save(Color? obj)
        {
            if (obj.HasValue)
            {
                var value = obj.Value;
                return new ColorSetting(
                    A: value.A,
                    R: value.R,
                    G: value.G,
                    B: value.B
                );
            } else
            {
                return null;
            }
        }

        public static Color? Load(ColorSetting? setting)
        {
            if (setting == null)
            {
                return null;
            }

            return Color.FromArgb(setting.A, setting.R, setting.G, setting.B);
        }
    }
}
