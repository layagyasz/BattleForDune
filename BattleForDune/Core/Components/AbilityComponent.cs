using BattleForDune.Core.Components.Abilities;
using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components
{
    public class AbilityComponent : IGameComponent<IAbility>
    {
        private readonly SparseStorage<IAbility> _abilities = new(1024);
        private readonly UniqueIndex _byId = new(1024);
        private readonly MultiIndex _byOwner = new(1024, 1024);

        public bool Add(IAbility ability)
        {
            if (_abilities.Allocate(ability, out var handle))
            {
                _byId.Add(ability.Id.Value, handle);
                _byOwner.Add(ability.OwnerId.Value, handle);
                return true;
            }
            return false;
        }

        public bool TryGet(EntityId id, out IAbility ability)
        {
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

        public ComponentEnumerator<SparseStorage<IAbility>, IAbility> GetEnumerator(EntityId ownerId)
        {
            return new(_abilities, _byOwner.GetEnumerator(ownerId.Value));
        }
    }
}
