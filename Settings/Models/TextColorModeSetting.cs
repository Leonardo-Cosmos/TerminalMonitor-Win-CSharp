/* 2022/5/8 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    static class TextColorModeSettings
    {
        private static readonly IReadOnlyDictionary<string, TextColorMode> textColorModeDict
            = InitTextColorDict();

        private static IReadOnlyDictionary<string, TextColorMode> InitTextColorDict()
        {
            Dictionary<string, TextColorMode> dict = new();
            var values = Enum.GetValues(typeof(TextColorMode));
            foreach (var item in values)
            {
                if (item is TextColorMode value)
                {
                    dict.Add(value.ToString(), value);
                }
            }

            return new ReadOnlyDictionary<string, TextColorMode>(dict);
        }

        static string TextColorModeToString(TextColorMode textColorMode) => textColorMode.ToString();

        static TextColorMode StringToTextColorMode(string str)
        {
            if (str != null && textColorModeDict.ContainsKey(str))
            {
                return textColorModeDict[str];
            }
            else
            {
                return TextColorMode.Static;
            }
        }

        public static string Save(TextColorMode? obj)
        {
            if (obj.HasValue)
            {
                return TextColorModeToString(obj.Value);
            }
            else
            {
                return null;
            }
        }

        public static TextColorMode? Load(string setting)
        {
            if (setting == null)
            {
                return null;
            }

            return StringToTextColorMode(setting);
        }
    }
}
