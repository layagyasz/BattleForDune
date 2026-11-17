using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class AttackChanceAddAuraHandler(
        RegistryKey AuraKey, sbyte Difficulty, byte Power, byte Range, sbyte Duration)
        : IAbilityHandler<AttackEvent>
    {
        public void Intercept(ref AttackEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(
            in AttackEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            ref var position = ref world.Positions.TryGet(@event.TargetId, out var isFound);
            if (!isFound)
            {
                return;
            }

            sink.Queue(
                AddAuraEvent.Transitive(
                    @event.SubjectId,
                    @event.AbilityId,
                    position,
                    AuraKey,
                    Power,
                    Range,
                    Duration,
                    skill: 0,
                    Difficulty));
        }
    }
}
