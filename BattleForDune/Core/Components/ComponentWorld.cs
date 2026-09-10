using BattleForDune.FastLookup;
using BattleForDune.Schema;
using Cardamom.Utils.Suppliers.Ids;
using OpenTK.Mathematics;

namespace BattleForDune.Core.Components
{
    public class ComponentWorld(LinearCongruentialIdSupplier ids)
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

        private readonly LinearCongruentialIdSupplier _ids = ids;

        private readonly IdSet _entities = new(1024);
        private readonly PositionComponent _positions = new();
        private readonly StatComponent _stats = new();
        private readonly ModifierComponent _modifiers = new();
        private readonly EffectComponent _effects = new();

        public void AddUnit(UnitType unitType, Vector2i position)
        {
            var id = new EntityId(_ids.Get());
            _entities.Add(id.Value);
            _positions.Add(id, position);
            _stats.Add(id, unitType.Stats);
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

        public void RemoveModifiers(EntityId sourceId)
        {
            _modifiers.RemoveBySource(sourceId, new RemoveModifierCallback(this));
        }
    }
}
