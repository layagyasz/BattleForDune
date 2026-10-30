using BattleForDune.Core.Components.Auras;
using BattleForDune.FastLookup;
using BattleForDune.Geometry;

namespace BattleForDune.Core.Components
{
    public class AuraComponent(HexRect2b bounds, int capacity) : IGameComponent<Hex2b>
    {
        private readonly ref struct InsertQuadTreeCallback(
            HexQuadTree<Aura> quadTree, Aura aura) : ICallback<Hex2b>
        {
            private readonly HexQuadTree<Aura> _quadTree = quadTree;
            private readonly Aura _aura = aura;

            public readonly void Notify(in Hex2b position)
            {
                _quadTree.Insert(position, _aura);
            }
        }

        private readonly ref struct RemoveQuadTreeCallback(
            HexQuadTree<Aura> quadTree, Aura aura) : ICallback<Hex2b>
        {
            private readonly HexQuadTree<Aura> _quadTree = quadTree;
            private readonly Aura _aura = aura;

            public readonly void Notify(in Hex2b position)
            {
                _quadTree.Remove(position, _aura);
            }
        }

        private readonly SparseStorage<Aura> _values = new(capacity);
        private readonly UniqueIndex _index = new(capacity);
        private readonly HexQuadTree<Aura> _quadTree =
            new(bounds, capacity >> 3, capacity, maxDepth: 6, maxItemsPerNode: 8);

        public bool Add(EntityId id, Aura value)
        {
            if (_values.Allocate(value, out var handle))
            {
                _index.Add(id.Value, handle);
                var callback = new InsertQuadTreeCallback(_quadTree, value);
                // TODO: Keep separate storage for high range auras
                Hexes.Range(value.Position, value.Range, ref callback);
                return true;
            }
            return false;
        }

        public void QueryRange<TCallback>(Hex2b position, byte range, ref TCallback callback)
            where TCallback : struct, ICallback<Aura>, allows ref struct
        {
            _quadTree.Query(position, range, ref callback);
        }

        public bool TryGet(EntityId id, out Aura value)
        {
            if (_index.TryGet(id.Value, out int handle))
            {
                value = _values.Get(handle);
                return true;
            }
            value = null!;
            return false;
        }

        public bool Remove<TCallback>(EntityId id, ref TCallback callback) 
            where TCallback : ICallback<Aura>, allows ref struct
        {
            if (_index.RemoveOnly(id.Value, out var handle))
            {
                var value = _values.Get(handle);
                callback.Notify(in value);
                var quadCallback = new RemoveQuadTreeCallback(_quadTree, value);
                Hexes.Range(value.Position, value.Range, ref quadCallback);
                _values.Free(handle);
                return true;
            }
            return false;
        }
    }
}
