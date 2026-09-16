using BattleForDune.Core.Events;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;
using Cardamom.Utils.Suppliers.Ids;

namespace BattleForDune.Core.Components
{
    public class ComponentWorld(
        ConfigurationRegistry registry, LinearCongruentialIdSupplier ids, Random random) : IGameEventRouter
    {
        private readonly ref struct RemoveModifierCallback(ComponentWorld world)
            : ModifierComponent.IModifierCallback
        {
            private readonly ComponentWorld _world = world;

            public readonly void Notify(EntityId ownerId, in StatBlock modifier)
            {
                ref var entityStats = ref _world._stats.TryGet(ownerId, out var isFound);
                if (isFound)
                {
                    entityStats.Remove(modifier);
                }
            }
        }

        private readonly ConfigurationRegistry _registry = registry;
        private readonly LinearCongruentialIdSupplier _ids = ids;
        private readonly Random _random = random;

        private readonly IdSet _units = new(1024);
        private readonly TeamComponent _teams = new(1024);
        private readonly FactionComponent _factions = new(8);
        private readonly PositionComponent _positions = new(1024);
        private readonly StatComponent _stats = new(1024);
        private readonly InitiativeComponent _initiative = new(1024);
        private readonly ModifierComponent _modifiers = new(1024);
        private readonly EffectComponent _effects = new(1024);
        private readonly AbilityComponent _abilities = new(1024);

        public EntityId NextId()
        {
            return new(_ids.Get());
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

        public byte Roll(Die die)
        {
            return die.Roll(_random);
        }

        public void Intercept<T>(ref T @event) where T : struct, IGameEvent
        {
            foreach (var effect in _effects.Enumerate(@event.Type))
            {
                ((IGameEventHandler<T>)effect).Intercept(ref @event, this);
            }
        }

        public void Handle<T>(in T @event, IGameEventSink sink) where T : struct, IGameEvent
        {
            foreach (var effect in _effects.Enumerate(@event.Type))
            {
                ((IGameEventHandler<T>)effect).Handle(in @event, this, sink);
            }
        }
    }
}
