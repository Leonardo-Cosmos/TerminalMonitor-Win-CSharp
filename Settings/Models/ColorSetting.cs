/* 2021/5/26 */
using System.Windows.Media;

namespace TerminalMonitor.Settings.Models
{
    record ColorSetting(byte A, byte R, byte G, byte B);

    static class ColorSettings
    {
        public static ColorSetting Save(Color obj)
        {
            return new ColorSetting(
                A: obj.A,
                R: obj.R,
                G: obj.G,
                B: obj.B
            );
        }

        public static Color Load(ColorSetting setting)
        {
            if (setting == null)
            {
                return Colors.Transparent;
            }

            return Color.FromArgb(setting.A, setting.R, setting.G, setting.B);
        }
    }
}
