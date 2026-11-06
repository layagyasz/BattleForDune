using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class UseAbilityAuraHandler(RegistryKey AuraKey, byte Power, byte Range, sbyte Duration)
        : IAbilityHandler<UseAbilityEvent>
    {
        public void Intercept(ref UseAbilityEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(
            in UseAbilityEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
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

            var aura =
                AddAuraEvent.Transitive(
                    @event.SubjectId, @event.AbilityId, position, AuraKey, Power, Range, Duration);
            sink.Queue(in aura);
        }
    }
}
