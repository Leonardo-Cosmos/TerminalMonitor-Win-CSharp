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
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.StackTrace);
                return new();
            }
        }

        public static TerminalLineVO ParseTerminalLineToVO(string json)
        {
            var dict = ParseTerminalLine(json);

            var parsedFields = dict.OrderBy(kvPair => kvPair.Key)
                .Select(kvPair => new TerminalLineFieldVO()
                {
                    Key = kvPair.Key,
                    Value = kvPair.Value.ToString(),
                })
                .ToList();

            return new TerminalLineVO()
            {
                PlainText = json,
                ParsedFieldDict = dict,
                ParsedFields = parsedFields,
            };
        }

    }
}
