using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public class CombatGameSystem(ComponentWorld world) : IGameEventProcessor
    {
        private readonly ComponentWorld _world = world;

        public void Process<TSink>(ref UseAbilityEvent @event, TSink sink) where TSink : IGameEventSink
        {
            Console.WriteLine("process ability");
            if (_world.TryGetAbility(@event.AbilityId, out var ability))
            {
                var handler = (IGameEventHandler<UseAbilityEvent>)ability;
                handler.Intercept(ref @event);
                @event.InstanceId = _world.GetId();
                handler.Handle(in @event, sink);
            }
        }

        public void Process<TSink>(ref AttackEvent @event, TSink sink) where TSink : IGameEventSink
        {
            Console.WriteLine($"Process attack");
        }
    }
}
