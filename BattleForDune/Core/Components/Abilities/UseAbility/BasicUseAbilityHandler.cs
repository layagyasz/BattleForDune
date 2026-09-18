using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using static BattleForDune.Core.Components.Abilities.IGenericHandler;

namespace BattleForDune.Core.Components.Abilities.UseAbility
{
    public class BasicUseAbilityHandler : IGameEventHandler<UseAbilityEvent>, IGenericHandler
    {
        public record class Prototype : IHandlerPrototype
        {
            public IGenericHandler Materialize()
            {
                return new BasicUseAbilityHandler();
            }
        }

        public void Intercept(ref UseAbilityEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(
            in UseAbilityEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            var attack =
                new AttackEvent(
                    @event.SubjectId,
                    @event.AbilityId,
                    @event.TargetId,
                    skill: 0,
                    difficulty: 0,
                    advantage: 0);
            sink.Queue(in attack);
        }
    }
}
