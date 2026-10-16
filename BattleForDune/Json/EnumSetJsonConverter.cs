using BattleForDune.FastLookup;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BattleForDune.Json
{
    public class EnumSetJsonConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }
            return typeToConvert.GetGenericTypeDefinition() == typeof(EnumSet<>);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var args = typeToConvert.GetGenericArguments();
            var converterArgs = new Type[] { args[0] };
            return (JsonConverter?)Activator.CreateInstance(
                typeof(EnumSetJsonConverterImpl<>).MakeGenericType(converterArgs));
        }

        class EnumSetJsonConverterImpl<TEnum>
            : JsonConverter<EnumSet<TEnum>> where TEnum : notnull, Enum
        {
            public override EnumSet<TEnum> Read(
                ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    return reader.GetString() switch
                    {
                        "$all" => EnumSet<TEnum>.All(),
                        _ => throw new JsonException($"Unsupported literal {reader.GetString()}"),
                    };
                }
                var result = (EnumSet<TEnum>)Activator.CreateInstance(typeToConvert)!;
                var values = JsonSerializer.Deserialize<List<TEnum>>(ref reader, options)!;
                foreach (var value in values)
                {
                    result.Add(value);
                }
                return result;
            }

            public override void Write(
                Utf8JsonWriter writer, EnumSet<TEnum> @object, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
