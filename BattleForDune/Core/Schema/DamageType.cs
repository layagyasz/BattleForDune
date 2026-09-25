using System.Text.Json.Serialization;

namespace BattleForDune.Core.Schema
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DamageType : byte
    {
        Caustic,
        Cold,
        Concussion,
        Heat,
        Physical,
        Poison,
        Psychic,
        Radiation,
        Shock
    }
}
