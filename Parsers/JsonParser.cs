/* 2021/4/20 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Windows.Controls;

namespace TerminalMonitor.Parsers
{
    static class JsonParser
    {
        public static Dictionary<string, object> ParseTerminalLine(string json)
        {
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                return dict ?? new();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.StackTrace);
                return new();
            }
        }

        public static Dictionary<string, object> FlattenJsonPath(Dictionary<string, object> dict)
        {
            var jsonProperties = dict.Select(kvPair => new
            {
                Key = kvPair.Key,
                Value = kvPair.Value,
            })
           .ToDictionary(kvPair => kvPair.Key, kvPair => kvPair.Value);

            return jsonProperties;
        }

    }
}
