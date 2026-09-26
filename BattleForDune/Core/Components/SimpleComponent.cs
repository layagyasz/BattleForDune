using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components
{
    public class SimpleComponent<T>(int capacity) : IGameComponent<T>
    {
        private static T _null = default;
        private readonly SparseStorage<T> _values = new(capacity);
        private readonly UniqueIndex _index = new(capacity);

        public bool Add(EntityId id, T value)
        {
            if (_values.Allocate(value, out var handle))
            {
                _index.Add(id.Value, handle);
                return true;
            }
            return false;
        }

        public ref T TryGet(EntityId id, out bool isFound)
        {
            if (_index.TryGet(id.Value, out int handle))
            {
                isFound = true;
                return ref _values.Get(handle);
            }
            isFound = false;
            return ref _null;
        }

        public bool Remove(EntityId id)
        {
            if (_index.RemoveOnly(id.Value, out var handle))
            {
                _values.Free(handle);
                return true;
            }
            return false;
        }
    }
}
