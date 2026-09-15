using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components
{
    public class ModifierComponent(int capacity) : IGameComponent<StatBlock>
    {
        public interface IModifierCallback
        {
            void Notify(EntityId ownerId, in StatBlock modifier);
        }

        private readonly record struct ModifierEntry(EntityId OwnerId, EntityId SourceId, StatBlock Modifier);

        private readonly DenseStorage<ModifierEntry> _values = new(capacity);
        private readonly MultiIndex _byOwner = new(capacity, capacity);
        private readonly MultiIndex _bySource = new(capacity, capacity);

        public bool Add(EntityId ownerId, EntityId sourceId, StatBlock modifier)
        {
            if (_values.Allocate(new(ownerId, sourceId, modifier), out var handle))
            {
                _byOwner.Add(ownerId.Value, handle);
                _bySource.Add(sourceId.Value, handle);
                return true;
            }
            return false;
        }

        public StatBlock TryGet(EntityId ownerId)
        {
            StatBlock result = new();
            foreach (var handle in _byOwner.GetEnumerator(ownerId.Value))
            {
                result.Add(_values.Get(handle).Modifier);
            }
            return result;
        }

        public void RemoveByOwner<T>(EntityId ownerId, T callback) 
            where T : struct, IModifierCallback, allows ref struct
        {
            foreach (var handle in _byOwner.GetEnumerator(ownerId.Value))
            {
                ref var value = ref _values.Get(handle);
                callback.Notify(ownerId, value.Modifier);
                _bySource.Remove(value.SourceId.Value, handle);
                _values.Free(handle);
            }
            _byOwner.RemoveAll(ownerId.Value);
        }

        public void RemoveBySource<T>(EntityId sourceId, T callback)
            where T : struct, IModifierCallback, allows ref struct
        {
            foreach (var handle in _bySource.GetEnumerator(sourceId.Value))
            {
                ref var value = ref _values.Get(handle);
                callback.Notify(value.SourceId, value.Modifier);
                _byOwner.Remove(value.OwnerId.Value, handle);
                _values.Free(handle);
            }
            _bySource.RemoveAll(sourceId.Value);
        }
    }
}
