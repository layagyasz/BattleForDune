using BattleForDune.Core.Events;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;
using Cardamom.Utils.Suppliers.Ids;

namespace BattleForDune.Core.Components
{
    public class ComponentWorld(
        LinearCongruentialIdSupplier ids, ConfigurationRegistry registry, Random random) : IGameEventRouter
    {
        private readonly LinearCongruentialIdSupplier _ids = ids;

        public ConfigurationRegistry Registry { get; init; } = registry;
        public Random Random { get; init; } = random;

        public byte Turn { get; private set; }
        public IdSet Units { get; init; } = new(1024);
        public TeamComponent Teams { get; init; } = new(1024);
        public FactionComponent Factions { get; init; } = new(1024);
        // TODO: Pass size in from map
        public PositionComponent Positions { get; init; } = new(new(new(0, 0), new(64, 32)), 1024);
        public BehaviorComponent Behaviors { get; init; } = new(1024);
        public StatComponent Stats { get; init; } = new(1024);
        public InitiativeComponent Inititiative { get; init; } = new(1024);
        public EffectComponent Effects { get; init; } = new(registry, 1024);
        public AbilityComponent Abilities { get; init; } = new(1024);
        public TimerComponent Timers { get; init; } = new(1024);

        public EntityId NextId()
        {
            var id = _ids.Get();
            if (id == 0)
            {
                id = _ids.Get();
            }
            return new(id);
        }

        public byte IncrementTurn()
        {
            return ++Turn;
        }

        public byte Roll(Die die)
        {
            return die.Roll(Random);
        }

        public void Intercept<T>(ref T @event) where T : struct, IGameEvent
        {
            foreach (var effect in Effects.ByEventType(@event.Type))
            {
                effect.Intercept(ref @event, this);
            }
        }

        public void Handle<T>(in T @event, IGameEventSink sink) where T : struct, IGameEvent
        {
            foreach (var effect in Effects.ByEventType(@event.Type))
            {
                effect.Handle(in @event, this, sink);
            }
        }
    }
}
