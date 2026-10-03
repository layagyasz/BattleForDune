using BattleForDune.Core.Components.Effects.Handlers;
using BattleForDune.Core.Events;
using System.Text.Json.Serialization;

namespace BattleForDune.Core.Components.Effects
{
    [JsonDerivedType(typeof(DamageImmunityHandler), "DamageImmunity")]
    [JsonDerivedType(typeof(DamageInAdvantageHandler), "DamageInAdvantage")]
    [JsonDerivedType(typeof(DamageOutAdvantageHandler), "DamageOutAdvantage")]
    [JsonDerivedType(typeof(DamageOverTimeHandler), "DamageOverTime")]
    [JsonDerivedType(typeof(UpdateStatHandler), "UpdateStat")]
    public interface IGenericEffectHandler
    {
        void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink) where TSink : IGameEventSink;
        void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink) where TSink : IGameEventSink;
    }
}
