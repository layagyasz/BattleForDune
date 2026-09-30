using System.Text.Json.Serialization;

namespace BattleForDune.Core.Events
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameEventType : byte
    {
        AddEffect,
        AddUnit,
        Attack,
        Damage,
        FinalizeInitiative,
        Initiative,
        Kill,
        RemoveEffect,
        Tick,
        UpdateStat,
        UseAbility
    }
}
