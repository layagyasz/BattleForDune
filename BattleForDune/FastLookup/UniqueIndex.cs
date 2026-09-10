using System.Numerics;

namespace BattleForDune.FastLookup
{
    public class UniqueIndex : IIndex
    {
        private record struct IndexEntry(int Key, int Handle);

        private readonly IndexEntry[] _table;
        private readonly int _mask;

        public UniqueIndex(int capacity)
        {
            capacity = (int)BitOperations.RoundUpToPowerOf2((uint)capacity);

            _mask = capacity - 1;
            _table = new IndexEntry[capacity];
            for (int i=0; i<capacity;++i)
            {
                _table[i].Handle = -1;
            }
        }

        public void Add(int key, int handle)
        {
            int index = key & _mask;
            while (_table[index].Handle != -1)
            {
                if (_table[index].Key == key)
                {
                    throw new InvalidOperationException($"Unique constraint violation for key: {key}");
                }
                index = (index + 1) & _mask;
            }
            _table[index].Key = key;
            _table[index].Handle = handle;
        }

        public bool TryGet(int key, out int handle)
        {
            int index = key & _mask;
            handle = -1;
            while (_table[index].Handle != -1)
            {
                if (_table[index].Key == key)
                {
                    handle = _table[index].Handle;
                    return true;
                }
                index = (index + 1) & _mask;
            }
            return false;
        }

        public bool RemoveOnly(int key, out int handle)
        {
            int index = key & _mask;

            ref var entry = ref _table[index];
            while (entry.Handle != -1)
            {
                if (entry.Key == key)
                {
                    handle = entry.Handle;
                    Rehash(index);
                    return true;
                }
                index = (index + 1) & _mask;
            }
            handle = -1;
            return false;
        }

        public void RemoveAll(int key)
        {
            RemoveOnly(key, out var _);
        }

        public void Remove(int key, int handle)
        {
            RemoveAll(key);
        }

        private void Rehash(int gapIndex)
        {
            int currentIndex = (gapIndex + 1) & _mask;

            while (_table[currentIndex].Handle != -1)
            {
                int idealIndex = _table[currentIndex].Key & _mask;

                bool cannotMove = (currentIndex >= gapIndex)
                    ? (idealIndex > gapIndex && idealIndex <= currentIndex)
                    : (idealIndex > gapIndex || idealIndex <= currentIndex);

                if (!cannotMove)
                {
                    _table[gapIndex] = _table[currentIndex];
                    gapIndex = currentIndex;
                }

                currentIndex = (currentIndex + 1) & _mask;
            }

            _table[gapIndex].Handle = -1;
        }

    }
}
