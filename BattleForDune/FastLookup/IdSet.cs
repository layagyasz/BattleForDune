using System.Numerics;

namespace BattleForDune.FastLookup
{
    public class IdSet
    {
        private record struct SetEntry(int Key, int Index);

        private readonly int[] _keys;
        private readonly SetEntry[] _values;
        private readonly int _mask;
        private int _count;

        public ReadOnlySpan<int> AsSpan() => _keys.AsSpan(0, _count);
        public int Count => _count;

        public IdSet(int capacity)
        {
            capacity = (int)BitOperations.RoundUpToPowerOf2((uint)capacity);

            _keys = new int[capacity];
            _values = new SetEntry[capacity];
            for (int i = 0; i < capacity; i++)
            {
                _values[i].Index = -1;
            }
            _mask = capacity - 1;
            _count = 0;
        }

        public bool Add(int val)
        {
            if (Contains(val))
            {
                return false;
            }

            if (_count >= _keys.Length)
            {
                throw new InvalidOperationException("Set is full.");
            }

            int slot = FindSlotForInsert(val);
            _values[slot].Key = val;
            _values[slot].Index = _count;

            _keys[_count] = val;
            _count++;
            return true;
        }

        public bool Remove(int val)
        {
            int slot = FindSlot(val);
            if (slot == -1)
            {
                return false;
            }

            int denseIndex = _values[slot].Index;
            int lastVal = _keys[_count - 1];

            RehashCluster(slot);

            _count--;
            if (denseIndex != _count)
            {
                _keys[denseIndex] = lastVal;
                int lastValSlot = FindSlot(lastVal);
                _values[lastValSlot].Index = denseIndex;
            }

            _keys[_count] = 0;

            return true;
        }

        public bool Contains(int val) => FindSlot(val) != -1;

        private int FindSlot(int val)
        {
            int slot = val & _mask;
            while (_values[slot].Index != -1)
            {
                if (_values[slot].Key == val) return slot;
                slot = (slot + 1) & _mask;
            }
            return -1;
        }

        private int FindSlotForInsert(int val)
        {
            int slot = val & _mask;
            while (_values[slot].Index != -1)
            {
                slot = (slot + 1) & _mask;
            }
            return slot;
        }

        private void RehashCluster(int gapIndex)
        {
            int current = (gapIndex + 1) & _mask;

            while (_values[current].Index != -1)
            {
                int ideal = _values[current].Key & _mask;
                bool cannotMove = (current >= gapIndex)
                    ? (ideal > gapIndex && ideal <= current)
                    : (ideal > gapIndex || ideal <= current);

                if (!cannotMove)
                {
                    _values[gapIndex] = _values[current];
                    gapIndex = current;
                }

                current = (current + 1) & _mask;
            }

            _values[gapIndex].Index = -1;
            _values[gapIndex].Key = 0;
        }
    }
}
