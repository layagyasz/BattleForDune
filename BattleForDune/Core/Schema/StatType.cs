using System.Text.Json.Serialization;

namespace BattleForDune.Core.Schema
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StatType : byte
    {
        Attack,
        Defense,
        Armor,
        Shield,
        Initiative,
        Hitpoints,
        Morale,
        Threat
    }
}
