using System.Runtime.CompilerServices;

namespace BattleForDune.FastLookup
{
    public class SparseStorage<T>(int capacity) : IStorage<T>
    {
        private readonly T[] _values = new T[capacity];
        private readonly int[] _nextHandle = new int[capacity];

        private int _freeHandle = -1;
        private int _maxHandle = 0;
        private int _count = 0;
        public int Count => _count;

        public ReadOnlySpan<T> AsSpan() => _values.AsSpan(0, _maxHandle);

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

            _count++;
            _values[handle] = value;

            return true;
        }

        public bool Free(int handle)
        {
            _values[handle] = default!;
            RecycleHandle(handle);
            _count--;
            return true;
        }

        public void FreeAll<TPredicate, TCallback>(in TPredicate predicate, in TCallback callback)
            where TPredicate : struct, IPredicate<T>, allows ref struct
            where TCallback : struct, ICallback<T>, allows ref struct
        {
            for (int handle = _count - 1; handle >= 0; handle--)
            {
                ref var value = ref _values[handle];
                if (predicate.Matches(in value))
                {
                    callback.Notify(handle, ref value);
                    _values[handle] = default!;
                    RecycleHandle(handle);
                }
            }
        }

        public ref T Get(int handle)
        {
            return ref _values[handle];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RecycleHandle(int handle)
        {
            _nextHandle[handle] = _freeHandle;
            _freeHandle = handle;
        }
    }

}
