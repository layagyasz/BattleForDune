using System.Collections;
using System.Numerics;

namespace BattleForDune.FastLookup
{
    public class MultiIndex : IIndex
    {
        private record struct IndexNode(int Next, int Previous, int Key, int Handle);

        public ref struct IndexEnumerator(MultiIndex index, int start, int targetKey) : IEnumerator<int>
        {
            private readonly MultiIndex _index = index;
            private readonly int _targetKey = targetKey;
            private int _current = start;
            private int _currentHandle = -1;

            public readonly int Current => _currentHandle;
            readonly object IEnumerator.Current => _currentHandle;

            public readonly IndexEnumerator GetEnumerator() => this;

            public readonly void Dispose() { }

            public bool MoveNext()
            {
                while (_current != -1)
                {
                    ref readonly var node = ref _index._nodes[_current];
                    _current = node.Next;
                    if (node.Key == _targetKey)
                    {
                        _currentHandle = node.Handle;
                        return true;
                    }
                }
                return false;
            }

            public readonly void Reset()
            {
                throw new NotSupportedException();
            }
        }

        private readonly IndexNode[] _nodes;
        private readonly int[] _heads;
        private readonly int _mask;

        public MultiIndex(int capacity, int buckets)
        {
            capacity = (int)BitOperations.RoundUpToPowerOf2((uint)capacity);
            buckets = (int)BitOperations.RoundUpToPowerOf2((uint)buckets);

            _mask = buckets - 1;
            _nodes = new IndexNode[capacity];
            _heads = new int[buckets];
            Array.Fill(_heads, -1);
        }

        public IndexEnumerator GetEnumerator(int key)
        {
            int bucket = key & _mask;
            return new IndexEnumerator(this, _heads[bucket], key);
        }

        public void Add(int key, int handle)
        {
            int bucket = key & _mask;
            int currentHead = _heads[bucket];

            ref var node = ref _nodes[handle];
            node.Key = key;
            node.Handle = handle;
            node.Next = currentHead;
            node.Previous = -1;

            if (currentHead != -1)
            {
                ref var current = ref _nodes[currentHead];
                current.Previous = handle;
            }
            _heads[bucket] = handle;
        }

        public void RemoveAll(int key)
        {
            int bucket = key & _mask;
            int current = _heads[bucket];
            while (current != -1)
            {
                ref var node = ref _nodes[current];
                int next = node.Next;
                if (node.Key == key)
                {
                    Unlink(bucket, ref node);
                }
                current = next;
            }
        }

        public void Remove(int key, int handle)
        {
            ref var node = ref _nodes[handle];
            int bucket = node.Key & _mask;
            Unlink(bucket, ref node);
        }

        private void Unlink(int bucket, ref IndexNode node)
        {
            if (node.Previous != -1)
            {
                ref var previous = ref _nodes[node.Previous];
                previous.Next = node.Next;
            }
            else
            {
                _heads[bucket] = node.Next;
            }
            if (node.Next != -1)
            {
                ref var next = ref _nodes[node.Next];
                next.Previous = node.Previous;
            }
            node = default;
        }
    }
}
