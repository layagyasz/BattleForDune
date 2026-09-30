using Cardamom.Json;
using System.Text.Json.Serialization;

namespace BattleForDune.Core.Schema
{
    [Flags]
    [JsonConverter(typeof(FlagJsonConverter<DamageVector>))]
    public enum DamageVector : byte
    {
        None = 0,
        Direct = 1,
        Indirect = 2,
        Evironmental = 4,
        All = 7
    }
}
