/* 2022/5/8 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    static class TextColorModeSettings
    {
        private static readonly ReadOnlyDictionary<string, TextColorMode> textColorModeDict
            = InitTextColorDict();

        private static ReadOnlyDictionary<string, TextColorMode> InitTextColorDict()
        {
            Dictionary<string, TextColorMode> dict = [];
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
            if (str != null && textColorModeDict.TryGetValue(str, out TextColorMode value))
            {
                return value;
            }
            else
            {
                return TextColorMode.Static;
            }
        }

        public static string? Save(TextColorMode? obj)
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

        public static TextColorMode? Load(string? setting)
        {
            if (setting == null)
            {
                return null;
            }

            return StringToTextColorMode(setting);
        }
    }
}
