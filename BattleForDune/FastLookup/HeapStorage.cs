using System.Runtime.CompilerServices;

namespace BattleForDune.FastLookup
{
    public class HeapStorage<TKeyer, TValue> : IStorage<TValue>
            where TKeyer : struct, IKeyer<TValue>
    {
        private readonly TKeyer _keyer;
        private readonly TValue[] _values;
        private readonly int[] _heap;
        private readonly int[] _handleToHeap;
        private readonly int[] _nextHandle;

        private int _freeHandle = -1;
        private int _maxHandle = 0;
        private int _count = 0;

        public int Count => _count;

        public HeapStorage(TKeyer keyer, int capacity)
        {
            _keyer = keyer;
            _values = new TValue[capacity];
            _heap = new int[capacity];
            _handleToHeap = new int[capacity];
            _nextHandle = new int[capacity];

            Array.Fill(_handleToHeap, -1);
        }

        public bool Allocate(TValue value, out int handle)
        {
            if (_freeHandle != -1)
            {
                handle = _freeHandle;
                _freeHandle = _nextHandle[handle];
            }
            else
            {
                handle = _maxHandle++;
                if (handle >= _values.Length)
                {
                    return false;
                }
            }

            _values[handle] = value;
            int index = _count++;

            _heap[index] = handle;
            _handleToHeap[handle] = index;

            SiftUp(index);
            return true;
        }

        public ref TValue Get(int handle)
        {
            return ref _values[_handleToHeap[handle]];
        }

        public ref TValue Peek()
        {
            return ref _values[_heap[0]];
        }

        public TValue PopMin(out int handle)
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("Empty heap");
            }

            handle = _heap[0];
            TValue result = _values[handle];
            Free(handle);

            return result;
        }

        public bool Free(int handle)
        {
            int index = _handleToHeap[handle];
            if (index == -1)
            {
                return false;
            }

            RemoveAtHeapIndex(index);

            _values[handle] = default!;
            _handleToHeap[handle] = -1;
            _nextHandle[handle] = _freeHandle;
            _freeHandle = handle;
            return true;
        }

        public void FreeAll<TPredicate, TCallback>(in TPredicate predicate, in TCallback callback)
            where TPredicate : struct, IPredicate<TValue>, allows ref struct
            where TCallback : struct, ICallback<TValue>, allows ref struct
        {
            for (int i = _count - 1; i >= 0; i--)
            {
                ref var value = ref _values[i];
                if (predicate.Matches(in value))
                {
                    int handle = _heap[i];
                    callback.Notify(handle, ref value);
                    RemoveAtHeapIndex(i);
                }
            }
        }

        private void RemoveAtHeapIndex(int index)
        {
            int lastIndex = --_count;

            if (index < lastIndex)
            {
                Swap(index, lastIndex);

                if (!SiftDown(index))
                {
                    SiftUp(index);
                }
            }
            _heap[lastIndex] = 0;
        }

        private void SiftUp(int index)
        {
            int handle = _heap[index];
            int key = _keyer.GetKey(in _values[handle]);

            while (index > 0)
            {
                int parentIndex = (index - 1) >> 1;
                int parentHandle = _heap[parentIndex];

                if (_keyer.GetKey(in _values[parentHandle]) <= key)
                {
                    break;
                }

                _heap[index] = parentHandle;
                _handleToHeap[parentHandle] = index;
                index = parentIndex;
            }

            _heap[index] = handle;
            _handleToHeap[handle] = index;
        }

        private bool SiftDown(int index)
        {
            int handle = _heap[index];
            bool moved = false;

            while (true)
            {
                int left = (index << 1) + 1;
                int right = left + 1;
                int smallIndex = index;

                if (left < _count
                    && _keyer.GetKey(in _values[_heap[left]]) < _keyer.GetKey(in _values[_heap[smallIndex]]))
                {
                    smallIndex = left;
                }

                if (right < _count
                    && _keyer.GetKey(in _values[_heap[right]]) < _keyer.GetKey(in _values[_heap[smallIndex]]))
                {
                    smallIndex = right;
                }

                if (smallIndex == index)
                {
                    break;
                }

                int childHandle = _heap[smallIndex];
                _heap[index] = childHandle;
                _handleToHeap[childHandle] = index;

                index = smallIndex;
                moved = true;
            }

            _heap[index] = handle;
            _handleToHeap[handle] = index;
            return moved;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Swap(int left, int right)
        {
            int leftHandle = _heap[left];
            int rightHandle = _heap[right];

            _heap[left] = rightHandle;
            _heap[right] = leftHandle;

            _handleToHeap[leftHandle] = right;
            _handleToHeap[rightHandle] = left;
        }
    }

}
