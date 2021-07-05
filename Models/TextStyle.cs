/* 2021/5/22 */
using System;
using System.Windows.Media;

namespace TerminalMonitor.Models
{
    public class TextStyle : ICloneable
    {
        public static TextStyle Empty => new()
        {
            Foreground = Colors.Black,
            Background = Colors.White,
        };

        public Color Foreground { get; set; }

        public Color Background { get; set; }

        public object Clone()
        {
            return new TextStyle()
            {
                Foreground = this.Foreground,
                Background = this.Background,
            };
        }
    }
}
