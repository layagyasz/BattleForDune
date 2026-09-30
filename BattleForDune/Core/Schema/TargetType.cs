using Cardamom.Json;
using System.Text.Json.Serialization;

namespace BattleForDune.Core.Schema
{
    [Flags]
    [JsonConverter(typeof(FlagJsonConverter<TargetType>))]
    public enum TargetType : byte
    {
        None = 0,
        Self = 1,
        Friend = 2,
        Enemy = 4
    }
}
