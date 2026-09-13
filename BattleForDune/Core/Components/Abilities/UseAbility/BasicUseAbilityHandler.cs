using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Components.Abilities.UseAbility
{
    public class BasicUseAbilityHandler : IGameEventHandler<UseAbilityEvent>, IGenericHandler
    {
        public void Intercept(ref UseAbilityEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(
            in UseAbilityEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            ref var stats = ref world.TryGetStats(@event.SubjectId, out bool isFound);
            if (!isFound)
            {
                return;
            }
            ref var targetStats = ref world.TryGetStats(@event.TargetId, out isFound);
            if (!isFound)
            {
                return;
            }

            var attack =
                new AttackEvent(
                    @event.SubjectId,
                    @event.AbilityId,
                    @event.TargetId,
                    @event.InstanceId,
                    stats.Attack.Value, 
                    stats.Defense.Value);
            sink.Queue(in attack);
        }
    }
}
