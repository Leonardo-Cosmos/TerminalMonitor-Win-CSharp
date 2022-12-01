/* 2022/5/8 */

using System;
using System.Windows.Media;

namespace TerminalMonitor.Models
{
    public class TextColorConfig : ICloneable
    {
        public TextColorMode Mode { get; set; }

        public Color? Color { get; set; }

        public object Clone()
        {
            return new TextColorConfig()
            {
                Mode = this.Mode,
                Color = this.Color,
            };
        }
    }
}
