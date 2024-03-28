/* 2024/3/25 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace TerminalMonitor.Settings.Models
{
    static class TextWrappingSettings
    {
        private static readonly IReadOnlyDictionary<string, TextWrapping> textWrappingDict
            = InitTextWrappingDict();

        private static IReadOnlyDictionary<string, TextWrapping> InitTextWrappingDict()
        {
            Dictionary<string, TextWrapping> dict = new();
            var values = Enum.GetValues(typeof(TextWrapping));
            foreach (var item in values)
            {
                if (item is TextWrapping value)
                {
                    dict.Add(value.ToString(), value);
                }
            }

            return new ReadOnlyDictionary<string, TextWrapping>(dict);
        }

        static string TextWrappingToString(TextWrapping textWrapping) => textWrapping.ToString();

        static TextWrapping StringToTextWrapping(string str)
        {
            if (str != null && textWrappingDict.ContainsKey(str))
            {
                return textWrappingDict[str];
            }
            else
            {
                return TextWrapping.NoWrap;
            }
        }

        public static string Save(TextWrapping? obj)
        {
            if (obj.HasValue)
            {
                return TextWrappingToString(obj.Value);
            }
            else
            {
                return null;
            }
        }

        public static TextWrapping? Load(string setting)
        {
            if (setting == null)
            {
                return null;
            }

            return StringToTextWrapping(setting);
        }
    }
}
