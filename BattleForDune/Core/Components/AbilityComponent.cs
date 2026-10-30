using BattleForDune.Core.Components.Abilities;
using BattleForDune.FastLookup;
using System.Diagnostics.CodeAnalysis;

namespace BattleForDune.Core.Components
{
    public class AbilityComponent(int capacity) : IGameComponent<Ability>
    {
        private readonly SparseStorage<Ability> _abilities = new(capacity);
        private readonly UniqueIndex _byId = new(capacity);
        private readonly MultiIndex _byOwner = new(capacity, capacity);

        public bool Add(Ability ability)
        {
            if (_abilities.Allocate(ability, out var handle))
            {
                _byId.Add(ability.Id.Value, handle);
                _byOwner.Add(ability.OwnerId.Value, handle);
                return true;
            }
            return false;
        }

        public ComponentEnumerator<SparseStorage<Ability>, Ability, MultiIndex.IndexEnumerator>
            ByOwner(EntityId ownerId)
        {
            return new(_abilities, _byOwner.GetEnumerator(ownerId.Value));
        }

        public bool TryGet(EntityId id, [MaybeNullWhen(false)] out Ability ability)
        {
            if (id == EntityId.None)
            {
                ability = default;
                return false;
            }
            if (_byId.TryGet(id.Value, out var handle))
            {
                ability = _abilities.Get(handle);
                return true;
            }
            ability = default;
            return false;
        }

        public bool Remove(EntityId id)
        {
            if (_byId.RemoveOnly(id.Value, out var handle))
            {
                ref var ability = ref _abilities.Get(handle);
                _byOwner.Remove(ability.OwnerId.Value, handle);
                _abilities.Free(handle);
                return true;
            }
            return false;
        }
    }
}
