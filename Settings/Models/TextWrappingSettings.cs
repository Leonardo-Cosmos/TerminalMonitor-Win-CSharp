/* 2024/3/25 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace TerminalMonitor.Settings.Models
{
    static class TextWrappingSettings
    {
        private static readonly ReadOnlyDictionary<string, TextWrapping> textWrappingDict
            = InitTextWrappingDict();

        private static ReadOnlyDictionary<string, TextWrapping> InitTextWrappingDict()
        {
            Dictionary<string, TextWrapping> dict = [];
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
            if (str != null && textWrappingDict.TryGetValue(str, out TextWrapping value))
            {
                return value;
            }
            else
            {
                return TextWrapping.NoWrap;
            }
        }

        public static string? Save(TextWrapping? obj)
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

        public static TextWrapping? Load(string? setting)
        {
            if (setting == null)
            {
                return null;
            }

            return StringToTextWrapping(setting);
        }
    }
}
