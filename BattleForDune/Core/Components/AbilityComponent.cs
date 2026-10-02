using BattleForDune.Core.Components.Abilities;
using BattleForDune.FastLookup;
using System.Diagnostics.CodeAnalysis;

namespace BattleForDune.Core.Components
{
    // TODO: Use inline array?
    public class AbilityComponent(int capacity) : IGameComponent<Ability[]>
    {
        private readonly SparseStorage<Ability[]> _abilities = new(capacity);
        private readonly UniqueIndex _byOwner = new(capacity);

        public bool Add(EntityId ownerId, Ability[] abilities)
        {
            if (_abilities.Allocate(abilities, out var handle))
            {
                _byOwner.Add(ownerId.Value, handle);
                return true;
            }
            return false;
        }

        public bool TryGet(EntityId ownerId, out Ability[] abilities)
        {
            if (_byOwner.TryGet(ownerId.Value, out var handle))
            {
                abilities = _abilities.Get(handle);
                return true;
            }
            abilities = [];
            return false;
        }

        public bool TryGet(EntityId ownerId, byte abilityId, [MaybeNullWhen(false)] out Ability ability)
        {
            if (abilityId == 0xFF)
            {
                ability = default;
                return false;
            }
            if (TryGet(ownerId, out var abilities))
            {
                ability = abilities[abilityId];
                return true;
            }
            ability = default;
            return false;
        }

        public bool Remove(EntityId ownerId)
        {
            if (_byOwner.RemoveOnly(ownerId.Value, out var handle))
            {
                _abilities.Free(handle);
                return true;
            }
            return false;
        }
    }
}
