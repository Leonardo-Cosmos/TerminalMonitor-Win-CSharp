/* 2021/8/1 */
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TerminalMonitor.Settings.Models
{
    class ConditionSettingConverter : JsonConverter<ConditionSetting>
    {
        private const string propertyNameMatchMode = "MatchMode";

        public override bool CanConvert(Type typeToConvert) => typeof(ConditionSetting).IsAssignableFrom(typeToConvert);

        public override ConditionSetting Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Utf8JsonReader readerClone = reader;

            if (readerClone.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            readerClone.Read();
            if (readerClone.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string propertyName = readerClone.GetString();
            if (propertyName == propertyNameMatchMode)
            {
                return JsonSerializer.Deserialize<GroupConditionSetting>(ref reader);
            }
            else
            {
                return JsonSerializer.Deserialize<FieldConditionSetting>(ref reader);
            }
        }

        public override void Write(Utf8JsonWriter writer, ConditionSetting value, JsonSerializerOptions options)
        {
            if (value is FieldConditionSetting fieldConditionSetting)
            {
                JsonSerializer.Serialize<FieldConditionSetting>(writer, fieldConditionSetting, options);
            }
            else if (value is GroupConditionSetting groupConditionSetting)
            {
                JsonSerializer.Serialize<GroupConditionSetting>(writer, groupConditionSetting, options);
            }
            else
            {
                throw new NotImplementedException("Unknown condition setting type");
            }
        }
    }
}
