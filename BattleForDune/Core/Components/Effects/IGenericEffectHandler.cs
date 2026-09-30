using BattleForDune.Core.Components.Effects.Handlers;
using BattleForDune.Core.Events;
using System.Text.Json.Serialization;

namespace BattleForDune.Core.Components.Effects
{
    public interface IGenericEffectHandler
    {
        void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink) where TSink : IGameEventSink;
        void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink) where TSink : IGameEventSink;

        [JsonDerivedType(typeof(DamageAdvantageHandler.Prototype), "DamageAdvantage")]
        [JsonDerivedType(typeof(DamageImmunityHandler.Prototype), "DamageImmunity")]
        [JsonDerivedType(typeof(DamageOverTimeHandler.Prototype), "DamageOverTime")]
        [JsonDerivedType(typeof(UpdateStatHandler.Prototype), "UpdateStat")]
        public interface IHandlerPrototype
        {
            IGenericEffectHandler Materialize();
        }
    }
}
