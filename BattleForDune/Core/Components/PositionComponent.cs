using BattleForDune.FastLookup;
using BattleForDune.Geometry;
using System.Runtime.CompilerServices;

namespace BattleForDune.Core.Components
{
    public class PositionComponent(HexRect2b bounds, int capacity) : IGameComponent<Hex2b>
    {
        private readonly SparseStorage<Hex2b> _values = new(capacity);
        private readonly UniqueIndex _index = new(capacity);
        private readonly HexQuadTree<int> _quadTree = 
            new(bounds, capacity >> 3, capacity, maxDepth: 6, maxItemsPerNode: 8);

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

        public bool IsOccupied(Hex2b position)
        {
            return _quadTree.IsOccupied(position);
        }

        public void QueryRange<TCallback>(Hex2b position, byte range, ref TCallback callback) 
            where TCallback : struct, ICallback<int>, allows ref struct
        {
            _quadTree.Query(position, range, ref callback);
        }

        public int QueryNearest<TPredicate>(Hex2b position, ref TPredicate predicate, out int nearest) 
            where TPredicate : IPredicate<int>, allows ref struct
        {
            return _quadTree.QueryNearest(position, ref predicate, out nearest);
        }

        public ref Hex2b TryGet(EntityId id, out bool isFound)
        {
            if (_index.TryGet(id.Value, out int handle))
            {
                isFound = true;
                return ref _values.Get(handle);
            }
            isFound = false;
            return ref Unsafe.NullRef<Hex2b>();
        }

        public bool Move(EntityId id, Hex2b value)
        {
            if (_index.TryGet(id.Value, out var handle))
            {
                ref var p = ref _values.Get(handle);
                var current = p;
                p = value;
                _quadTree.Remove(current, id.Value);
                _quadTree.Insert(p, id.Value);
                return true;
            }
            return false;
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
