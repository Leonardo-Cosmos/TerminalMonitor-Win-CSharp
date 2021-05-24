/* 2021/5/22 */
using System.Windows.Media;

namespace TerminalMonitor.Models
{
    public class TextStyle
    {
        private static readonly TextStyle defaultInstance = new()
        {
            Foreground = Colors.Black,
            Background = Colors.White,
        };

        public static TextStyle Default => defaultInstance;

        public Color Foreground { get; set; }

        public Color Background { get; set; }
    }
}
