using System.Runtime.CompilerServices;

namespace BattleForDune.FastLookup
{
    public class DenseStorage<T> : IStorage<T>
    {
        private readonly T[] _values;
        private readonly int[] _handleToIndex;
        private readonly int[] _indexToHandle;
        private readonly int[] _nextHandle;

        private int _freeHandle = -1;
        private int _maxHandle = 0;
        private int _count = 0;
        public int Count => _count;

        public DenseStorage(int capacity)
        {
            _values = new T[capacity];
            _handleToIndex = new int[capacity];
            _indexToHandle = new int[capacity];
            _nextHandle = new int[capacity];

            Array.Fill(_handleToIndex, -1);
            Array.Fill(_indexToHandle, -1);
        }

        public ReadOnlySpan<T> AsSpan() => _values.AsSpan(0, _count);

        public bool Allocate(T value, out int handle)
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

            int index = _count++;
            _values[index] = value;

            _handleToIndex[handle] = index;
            _indexToHandle[index] = handle;

            return true;
        }

        public bool Free(int handle)
        {
            var index = _handleToIndex[handle];
            if (index == -1)
            {
                return false;
            }

            SwapAndPop(index);
            RecycleHandle(handle);

            return true;
        }

        public void FreeAll<TPredicate, TCallback>(in TPredicate predicate, in TCallback callback)
            where TPredicate : struct, IPredicate<T>, allows ref struct
            where TCallback : struct, ICallback<T>, allows ref struct
        {
            for (int i = _count - 1; i >= 0; i--)
            {
                ref var value = ref _values[i];
                if (predicate.Matches(in value))
                {
                    int handle = _indexToHandle[i];
                    callback.Notify(handle, ref value);
                    SwapAndPop(i);
                    RecycleHandle(handle);
                }
            }
        }

        public ref T Get(int handle)
        {
            return ref _values[_handleToIndex[handle]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SwapAndPop(int indexToRemove)
        {
            int lastIndex = _count - 1;

            if (indexToRemove < lastIndex)
            {
                _values[indexToRemove] = _values[lastIndex];

                int movingHandle = _indexToHandle[lastIndex];
                _handleToIndex[movingHandle] = indexToRemove;
                _indexToHandle[indexToRemove] = movingHandle;
            }

            _values[lastIndex] = default;
            _indexToHandle[lastIndex] = -1;
            _count--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RecycleHandle(int handle)
        {
            _handleToIndex[handle] = -1;
            _nextHandle[handle] = _freeHandle;
            _freeHandle = handle;
        }
    }
}