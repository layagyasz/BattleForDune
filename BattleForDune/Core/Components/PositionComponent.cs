using BattleForDune.FastLookup;
using BattleForDune.Geometry;

namespace BattleForDune.Core.Components
{
    public class PositionComponent(HexRect2b bounds, int capacity) : IGameComponent<Hex2b>
    {
        private static Hex2b _null = default!;
        private readonly SparseStorage<Hex2b> _values = new(capacity);
        private readonly UniqueIndex _index = new(capacity);
        private readonly HexQuadTree<int> _quadTree = new(bounds, capacity >> 3, capacity);

        public bool Add(EntityId id, Hex2b value)
        {
            if (_values.Allocate(value, out var handle))
            {
                _index.Add(id.Value, handle);
                _quadTree.Insert(value, id.Value);
                return true;
            }
            return false;
        }

        public void QueryRange<TCallback>(Hex2b position, byte range, ref TCallback callback) 
            where TCallback : struct, ICallback<int>, allows ref struct
        {
            _quadTree.Query(position, range, ref callback);
        }

        public ref Hex2b TryGet(EntityId id, out bool isFound)
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
                _quadTree.Remove(_values.Get(handle), id.Value);
                _values.Free(handle);
                return true;
            }
            return false;
        }
    }
}
