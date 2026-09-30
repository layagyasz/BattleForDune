using BattleForDune.Core.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BattleForDune.Json
{
    public class DieJsonConverter : JsonConverter<Die>
    {
        public override Die Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString()!.AsSpan();
            var dSeparator = value.IndexOf('d');
            var aSeparator = value.IndexOf('+');
            var mSeparator = value.IndexOf('-');
            if (aSeparator >= 0 && mSeparator >= 0)
            {
                throw new JsonException($"Can either use + or - in {value}");
            }
            if (aSeparator >= 0)
            {
                return new(
                    byte.Parse(value[..dSeparator]), 
                    byte.Parse(value.Slice(dSeparator + 1, aSeparator - dSeparator - 1)), 
                    sbyte.Parse(value[(aSeparator + 1)..]),
                    0);
            }
            else if (mSeparator >= 0)
            {
                return new(
                    byte.Parse(value[..dSeparator]),
                    byte.Parse(value.Slice(dSeparator + 1, mSeparator - dSeparator - 1)),
                    sbyte.Parse(value[mSeparator..]),
                    0);
            }
            else return new(
                byte.Parse(value[..dSeparator]),
                byte.Parse(value[(dSeparator + 1)..]),
                0,
                0);
        }

        public override void Write(Utf8JsonWriter writer, Die @object, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
