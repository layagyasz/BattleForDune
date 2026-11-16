using BattleForDune.Core.Components.Effects.Handlers;
using BattleForDune.Core.Events;
using BattleForDune.Core.Schema;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace BattleForDune.Core.Components.Effects
{
    [JsonDerivedType(typeof(AddEffectHandler), "AddEffect")]
    [JsonDerivedType(typeof(AttackHandler), "Attack")]
    [JsonDerivedType(typeof(DamageByEffectsHandler), "DamageByEffects")]
    [JsonDerivedType(typeof(DamageChanceAddEffectHandler), "DamageChanceAddEffect")]
    [JsonDerivedType(typeof(DamageImmunityHandler), "DamageImmunity")]
    [JsonDerivedType(typeof(DamageModifierHandler), "DamageModifier")]
    [JsonDerivedType(typeof(DamageOverTimeHandler), "DamageOverTime")]
    [JsonDerivedType(typeof(EffectImmunityHandler), "EffectImmunity")]
    [JsonDerivedType(typeof(KillActionPointsHandler), "KillActionPoints")]
    [JsonDerivedType(typeof(KillSpawnHandler), "KillSpawn")]
    [JsonDerivedType(typeof(MassiveCriticalHandler), "MassiveCritical")]
    [JsonDerivedType(typeof(OverrideAllegianceHandler), "OverrideAllegiance")]
    [JsonDerivedType(typeof(ResetActionsHandler), "ResetActions")]
    [JsonDerivedType(typeof(SpawnAddEffectHandler), "SpawnAddEffect")]
    [JsonDerivedType(typeof(UpdateStatHandler), "UpdateStat")]
    public interface IGenericEffectHandler
    {
        void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink) where TSink : IGameEventSink;
        void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink) where TSink : IGameEventSink;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasDirection<T>(in T @event, AbilityDirection direction, EntityId ownerId)
            where T : struct, IGameEvent => direction switch
            {
                AbilityDirection.Incoming => ownerId == @event.TargetId,
                AbilityDirection.Outgoing => ownerId == @event.SubjectId,
                _ => throw new ArgumentException($"Unsupported AbilityDirection {direction}")
            };
    }
}
