using System.Text.Json.Serialization;

namespace BattleForDune.Core.Events
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameEventType : byte
    {
        AddEffect,
        AddUnit,
        Attack,
        Break,
        Damage,
        FinalizeInitiative,
        Initiative,
        Kill,
        Move,
        RemoveEffect,
        Tick,
        UpdateStat,
        UseAbility
    }
}
