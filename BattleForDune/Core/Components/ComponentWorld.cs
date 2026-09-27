using BattleForDune.Core.Events;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;
using Cardamom.Utils.Suppliers.Ids;

namespace BattleForDune.Core.Components
{
    public class ComponentWorld(
        ConfigurationRegistry registry, LinearCongruentialIdSupplier ids, Random random) : IGameEventRouter
    {
        private readonly ConfigurationRegistry _registry = registry;
        private readonly LinearCongruentialIdSupplier _ids = ids;
        private readonly Random _random = random;

        private byte _turn;
        private readonly IdSet _units = new(1024);
        private readonly TeamComponent _teams = new(1024);
        private readonly FactionComponent _factions = new(1024);
        private readonly PositionComponent _positions = new(1024);
        private readonly StatComponent _stats = new(1024);
        private readonly InitiativeComponent _initiative = new(1024);
        private readonly EffectComponent _effects = new(1024);
        private readonly AbilityComponent _abilities = new(1024);
        private readonly TimerComponent _timers = new(1024);

        public EntityId NextId()
        {
            var id = _ids.Get();
            if (id == 0)
            {
                id = _ids.Get();
            }
            return new(id);
        }

        public IdSet GetUnits()
        {
            return _units;
        }

        public AbilityComponent GetAbilities()
        {
            return _abilities;
        }

        public EffectComponent GetEffects()
        {
            return _effects;
        }

        public FactionComponent GetFactions()
        {
            return _factions;
        }

        public InitiativeComponent GetInitiative()
        {
            return _initiative;
        }

        public PositionComponent GetPositions()
        {
            return _positions;
        }

        public Random GetRandom()
        {
            return _random;
        }

        public ConfigurationRegistry GetRegistry()
        {
            return _registry;
        }

        public StatComponent GetStats()
        {
            return _stats;
        }

        public TeamComponent GetTeams()
        {
            return _teams;
        }

        public TimerComponent GetTimers()
        {
            return _timers;
        }

        public byte GetTurn()
        {
            return _turn;
        }

        public byte IncrementTurn()
        {
            return ++_turn;
        }

        public byte Roll(Die die)
        {
            return die.Roll(_random);
        }

        public void Intercept<T>(ref T @event) where T : struct, IGameEvent
        {
            foreach (var effect in _effects.ByEventType(@event.Type))
            {
                ((IGameEventHandler<T>)effect).Intercept(ref @event, this);
            }
        }

        public void Handle<T>(in T @event, IGameEventSink sink) where T : struct, IGameEvent
        {
            foreach (var effect in _effects.ByEventType(@event.Type))
            {
                ((IGameEventHandler<T>)effect).Handle(in @event, this, sink);
            }
        }
    }
}
