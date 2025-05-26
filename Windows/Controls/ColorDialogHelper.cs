/* 2023/3/3 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TerminalMonitor.Windows.Controls
{
    static class ColorDialogHelper
    {
        public static SolidColorBrush? ShowColorDialog(SolidColorBrush brush)
        {
            var color = brush.Color;
            System.Windows.Forms.ColorDialog colorDialog = new()
            {
                CustomColors = GetCustomColorsSetting(),
                Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)
            };

            SolidColorBrush? solidColorBrush = null;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var selectedColor = colorDialog.Color;
                color = Color.FromArgb(selectedColor.A,
                    selectedColor.R, selectedColor.G, selectedColor.B);
                solidColorBrush = new SolidColorBrush(color);

                SaveSelectedColor(colorDialog);
                SetCustomColorsSetting(colorDialog.CustomColors);
            }

            return solidColorBrush;
        }

        private static void SaveSelectedColor(System.Windows.Forms.ColorDialog colorDialog)
        {
            var color = colorDialog.Color;
            var selectedColor = color.R | (color.G << 8) | (color.B << 16);

            if (!colorDialog.CustomColors.Contains(selectedColor))
            {
                var customColors = colorDialog.CustomColors;

                bool added = false;
                for (int i = 0; i < customColors.Length; i++)
                {
                    if (customColors[i] == 0xffffff)
                    {
                        customColors[i] = selectedColor;
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    customColors = [.. customColors.Skip(1), selectedColor];
                }

                colorDialog.CustomColors = customColors;
            }
        }

        private static int[] GetCustomColorsSetting()
        {
            var customColors = Properties.WindowSettings.Default.CustomColors ??= [];
            var colors = new string[customColors.Count];
            customColors.CopyTo(colors, 0);

            return [.. colors.Select(colorStr => ConvertToInt32(colorStr))];
        }

        private static int ConvertToInt32(string value)
        {
            try
            {
                return Convert.ToInt32(value, 16);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return 0;
            }
        }

        private static void SetCustomColorsSetting(int[] customColors)
        {
            var colors = customColors.Select(colorInt => Convert.ToString(colorInt, 16)).ToArray();

            Properties.WindowSettings.Default.CustomColors.Clear();
            Properties.WindowSettings.Default.CustomColors.AddRange(colors);
        }
    }
}
