using BattleForDune.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BattleForDune.Json
{
    public class RegistryKeyJsonConverter : JsonConverter<RegistryKey>
    {
        public override RegistryKey Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new(reader.GetUInt16());
        }

        public override void Write(Utf8JsonWriter writer, RegistryKey @object, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(@object.Value);
        }
    }
}
