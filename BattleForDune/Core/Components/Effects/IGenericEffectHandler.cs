using BattleForDune.Core.Components.Effects.Handlers;
using BattleForDune.Core.Events;
using System.Text.Json.Serialization;

namespace BattleForDune.Core.Components.Effects
{
    [JsonDerivedType(typeof(AttackInHandler), "AttackIn")]
    [JsonDerivedType(typeof(AttackOutHandler), "AttackOut")]
    [JsonDerivedType(typeof(DamageImmunityHandler), "DamageImmunity")]
    [JsonDerivedType(typeof(DamageInModifierHandler), "DamageInModifier")]
    [JsonDerivedType(typeof(DamageOutModifierHandler), "DamageOutModifier")]
    [JsonDerivedType(typeof(DamageOverTimeHandler), "DamageOverTime")]
    [JsonDerivedType(typeof(MassiveCriticalHandler), "MassiveCritical")]
    [JsonDerivedType(typeof(UpdateStatHandler), "UpdateStat")]
    public interface IGenericEffectHandler
    {
        void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink) where TSink : IGameEventSink;
        void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink) where TSink : IGameEventSink;
    }
}
