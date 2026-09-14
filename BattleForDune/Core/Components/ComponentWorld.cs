using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Effects;
using BattleForDune.Core.Events;
using BattleForDune.Core.Prototypes.Abilities;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;
using Cardamom.Utils.Suppliers.Ids;
using OpenTK.Mathematics;

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
                    entityStats.Subtract(modifier);
                }
            }
        }

        private readonly ConfigurationRegistry _registry = registry;
        private readonly LinearCongruentialIdSupplier _ids = ids;
        private readonly Random _random = random;

        private readonly IdSet _units = new(1024);
        private readonly PositionComponent _positions = new();
        private readonly StatComponent _stats = new();
        private readonly InitiativeComponent _initiative = new();
        private readonly ModifierComponent _modifiers = new();
        private readonly EffectComponent _effects = new();
        private readonly AbilityComponent _abilities = new();

        public void AddUnit(EntityId prototypeId, Vector2i position)
        {
            var prototype = _registry.GetUnitPrototype(prototypeId);
            var id = new EntityId(_ids.Get());
            _units.Add(id.Value);
            _positions.Add(id, position);
            _stats.Add(id, prototype.Stats.Materialize());
            for (int i=0; i< prototype.Abilities.Length; ++i)
            {
                AddAbility(prototype.Abilities[i], id);
            }
        }

        public void AddAbility(IAbilityPrototype prototype, EntityId ownerId)
        {
            var ability = prototype.Materialize(GetId(), ownerId);
            _abilities.Add(ability);
        }

        public void AddEffect(EffectParameters @params)
        {
            _effects.Add(@params);
        }

        public void AddModifier(EntityId ownerId, EntityId sourceId, StatBlock modifier)
        {
            if (_modifiers.Add(ownerId, sourceId, modifier))
            {
                ref var stats = ref _stats.TryGet(ownerId, out bool isFound);
                if (isFound)
                {
                    stats.Add(modifier);
                }
            }
        }

        public ComponentEnumerator<SparseStorage<IAbility>, IAbility> GetAbilities(EntityId ownerId)
        {
            return _abilities.GetEnumerator(ownerId);
        }

        public bool TryGetAbility(EntityId id, out IAbility ability)
        {
            var result = _abilities.TryGet(id, out var container);
            ability = container;
            return result;
        }

        public EntityId GetId()
        {
            return new(_ids.Get());
        }

        public InitiativeComponent GetInitiative()
        {
            return _initiative;
        }

        public ref StatBlock TryGetStats(EntityId id, out bool isFound)
        {
            ref var result = ref _stats.TryGet(id, out bool found);
            isFound = found;
            return ref result;
        }

        public ReadOnlySpan<int> GetUnitIds()
        {
            return _units.AsSpan();
        }

        public byte Roll(Die die)
        {
            return die.Roll(_random);
        }

        public void RemoveModifiers(EntityId sourceId)
        {
            _modifiers.RemoveBySource(sourceId, new RemoveModifierCallback(this));
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
