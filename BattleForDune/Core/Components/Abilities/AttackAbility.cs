using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Components.Abilities
{
    public record class AttackAbility(EntityId Id, EntityId OwnerId) 
        : IAbility, IGameEventHandler<UseAbilityEvent>
    {
        public void Trigger<TSink>(in TSink sink) where TSink : IGameEventSink
        {
            var @event = new AttackEvent(Id, 0, 0);
            sink.Queue(in @event);
        }

        public void Intercept(ref UseAbilityEvent @event)
        {
            Console.WriteLine("intercepted my own event!");
        }

        public void Handle<TSink>(in UseAbilityEvent @event, in TSink sink) where TSink : IGameEventSink
        {
            Console.WriteLine("reacted my own event!");
        }
    }
}
