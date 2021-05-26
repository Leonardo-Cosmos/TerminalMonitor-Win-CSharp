/* 2021/5/22 */
using System.Windows.Media;

namespace TerminalMonitor.Models
{
    public class TextStyle
    {
        public static TextStyle Empty => new()
        {
            Foreground = Colors.Black,
            Background = Colors.White,
        };

        public Color Foreground { get; set; }

        public Color Background { get; set; }
    }
}
