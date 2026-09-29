using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace BattleForDune.FastLookup
{
    public class ComplexIndex : IIndex
    {
        public ref struct IndexEnumerator : IEnumerator<int>
        {
            private readonly ComplexIndex _index;
            private int _currentHandle;
            private int _currentEdge;

            public readonly int Current => _currentHandle;
            readonly object IEnumerator.Current => _currentHandle;

            public IndexEnumerator(ComplexIndex index, int key)
            {
                _index = index;
                int keySlot = index.FindKeySlot(key);
                _currentEdge = keySlot != -1 ? index._keys[keySlot].FirstEdge : -1;
                _currentHandle = -1;
            }

            public readonly void Dispose() { }

            public bool MoveNext()
            {
                if (_currentEdge == -1)
                {
                    return false;
                }
                _currentHandle = _index._edges[_currentEdge].ValueIndex;
                _currentEdge = _index._edges[_currentEdge].NextKeyEdge;
                return true;
            }

            public readonly void Reset()
            {
                throw new NotImplementedException();
            }

            public readonly IndexEnumerator GetEnumerator() => this;
        }

        private record struct KeySlot(int Key, int FirstEdge);

        private record struct ValueSlot(int FirstEdge);

        private record struct Edge(
            int KeyIndex,
            int ValueIndex, 
            int NextKeyEdge,
            int PreviousKeyEdge,
            int NextValueEdge,
            int PreviousValueEdge);

        private readonly KeySlot[] _keys;
        private readonly ValueSlot[] _values;
        private readonly Edge[] _edges;

        private readonly int _keyMask;
        private int _freeEdgeHead = -1;

        public ComplexIndex(int keyCapacity, int valueCapacity, int relationCapacity)
        {
            keyCapacity = (int)BitOperations.RoundUpToPowerOf2((uint)keyCapacity);
            _keyMask = keyCapacity - 1;

            _keys = new KeySlot[keyCapacity];
            _values = new ValueSlot[valueCapacity];
            _edges = new Edge[relationCapacity];

            for (int i = 0; i < keyCapacity; i++)
            {
                _keys[i].FirstEdge = -1;
            }
            for (int i = 0; i < valueCapacity; i++) 
            { 
                _values[i].FirstEdge = -1; 
            }

            for (int i = 0; i < relationCapacity; i++)
            {
                ref var edge = ref _edges[i];
                edge.ValueIndex = -1;
                edge.KeyIndex = -1;
                edge.NextKeyEdge = _freeEdgeHead;
                _freeEdgeHead = i;
            }
        }

        public void Add(int key, int valueIndex)
        {
            int keyIndex = GetOrAddKeySlot(key);
            if (keyIndex == -1)
            {
                throw new InternalBufferOverflowException();
            }

            if (EdgeExists(keyIndex, valueIndex))
            {
                return;
            }

            if (_freeEdgeHead == -1)
            {
                throw new InternalBufferOverflowException();
            }
            
            ref ValueSlot valSlot = ref _values[valueIndex];

            // Pop edge from free list
            int edgeIndex = _freeEdgeHead;
            ref Edge edge = ref _edges[edgeIndex];
            _freeEdgeHead = edge.NextKeyEdge;

            edge.KeyIndex = keyIndex;
            edge.ValueIndex = valueIndex;

            // Link into Key list
            ref KeySlot keySlot = ref _keys[keyIndex];
            edge.NextKeyEdge = keySlot.FirstEdge;
            edge.PreviousKeyEdge = -1;
            if (keySlot.FirstEdge != -1)
            {
                _edges[keySlot.FirstEdge].PreviousKeyEdge = edgeIndex;
            }
            keySlot.FirstEdge = edgeIndex;

            // Link into Value list
            edge.NextValueEdge = valSlot.FirstEdge;
            edge.PreviousValueEdge = -1;
            if (valSlot.FirstEdge != -1)
            {
                _edges[valSlot.FirstEdge].PreviousValueEdge = edgeIndex;
            }
            valSlot.FirstEdge = edgeIndex;
        }

        public IndexEnumerator GetEnumerator(int key) => new(this, key);

        public void Remove(int key, int handle)
        {
            int keyIndex = FindKeySlot(key);
            if (keyIndex == -1)
            {
                return;
            }

            int edgeIdx = FindEdge(keyIndex, handle);
            if (edgeIdx == -1)
            {
                return;
            }

            ref Edge edge = ref _edges[edgeIdx];

            // Unlink from Key list
            if (edge.PreviousKeyEdge != -1)
            {
                _edges[edge.PreviousKeyEdge].NextKeyEdge = edge.NextKeyEdge;
            }
            else
            {
                _keys[keyIndex].FirstEdge = edge.NextKeyEdge;
            }

            if (edge.NextKeyEdge != -1)
            {
                _edges[edge.NextKeyEdge].PreviousKeyEdge = edge.PreviousKeyEdge;
            }

            // Unlink from Value list
            if (edge.PreviousValueEdge != -1)
            {
                _edges[edge.PreviousValueEdge].NextValueEdge = edge.NextValueEdge;
            }
            else
            {
                _values[handle].FirstEdge = edge.NextValueEdge;
            }

            if (edge.NextValueEdge != -1)
            {
                _edges[edge.NextValueEdge].PreviousValueEdge = edge.PreviousValueEdge;
            }

            // Return edge to pool
            edge.NextKeyEdge = _freeEdgeHead;
            _freeEdgeHead = edgeIdx;
        }

        public void RemoveAll(int key)
        {
            int keySlotIdx = FindKeySlot(key);
            if (keySlotIdx == -1)
            {
                return;
            }

            ref KeySlot keySlot = ref _keys[keySlotIdx];
            int currentIndex = keySlot.FirstEdge;

            if (currentIndex == -1)
            {
                keySlot.Key = 0;
                keySlot.FirstEdge = -1;
                return;
            }

            // Traverse and disconnect all edges attached to this key
            while (currentIndex != -1)
            {
                ref Edge edge = ref _edges[currentIndex];
                int nextIndex = edge.NextKeyEdge;

                if (edge.PreviousValueEdge != -1)
                {
                    _edges[edge.PreviousValueEdge].NextValueEdge = edge.NextValueEdge;
                }
                else
                {
                    _values[edge.ValueIndex].FirstEdge = edge.NextValueEdge;
                }

                if (edge.NextValueEdge != -1)
                {
                    _edges[edge.NextValueEdge].PreviousValueEdge = edge.PreviousValueEdge;
                }

                edge.ValueIndex = -1;
                edge.KeyIndex = -1;

                // Return edge to the free pool
                edge.NextKeyEdge = _freeEdgeHead;
                _freeEdgeHead = currentIndex;

                currentIndex = nextIndex;
            }

            // Reset key slot links and mark as empty
            keySlot.FirstEdge = -1;
            keySlot.Key = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetOrAddKeySlot(int key)
        {
            for (int i = 0; i < _keys.Length; i++)
            {
                int slot = (key + i) & _keyMask;
                ref KeySlot k = ref _keys[slot];
                if (k.FirstEdge == -1)
                {
                    k.Key = key;
                    return slot;
                }
                if (k.Key == key)
                {
                    return slot;
                }
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindKeySlot(int key)
        {
            for (int i = 0; i < _keys.Length; i++)
            {
                int slot = (key + i) & _keyMask;
                ref KeySlot k = ref _keys[slot];

                if (k.FirstEdge == -1)
                {
                    return -1;
                }
                if (k.Key == key)
                {
                    return slot;
                }
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool EdgeExists(int keySlotIdx, int valueIndex) => FindEdge(keySlotIdx, valueIndex) != -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindEdge(int keyIndex, int valueIndex)
        {
            int curr = _values[valueIndex].FirstEdge;
            while (curr != -1)
            {
                ref Edge e = ref _edges[curr];
                if (e.KeyIndex == keyIndex)
                {
                    return curr;
                }
                curr = e.NextValueEdge;
            }
            return -1;
        }
    }
}