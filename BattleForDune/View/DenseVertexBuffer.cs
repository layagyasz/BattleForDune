using Cardamom;
using Cardamom.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.CompilerServices;

namespace BattleForDune.View
{
    public class DenseVertexBuffer<T> : ManagedResource where T : struct
    {
        private readonly int _blocks;
        private readonly int _blockSize;
        private readonly int _capacity;
        private readonly VertexBuffer<T> _vertices;
        private readonly int[] _handleToIndex;
        private readonly int[] _indexToHandle;
        private readonly int[] _nextHandle;

        private int _freeHandle = -1;
        private int _maxHandle = 0;
        private int _count = 0;
        public int Count => _count;

        public DenseVertexBuffer(int blocks, int blockSize)
        {
            _blocks = blocks;
            _blockSize = blockSize;
            _capacity = blocks * blockSize;
            _vertices = new VertexBuffer<T>(PrimitiveType.Triangles);
            _handleToIndex = new int[_blocks];
            _indexToHandle = new int[_blocks];
            _nextHandle = new int[_blocks];

            Array.Fill(_handleToIndex, -1);
            Array.Fill(_indexToHandle, -1);
        }

        protected override void DisposeImpl()
        {
            _vertices.Dispose();
        }

        public void Initialize()
        {
            _vertices.Allocate(_capacity);
        }

        public VertexBuffer<T> GetBuffer()
        {
            return _vertices;
        }

        public bool Allocate(out int handle)
        {
            if (_freeHandle != -1)
            {
                handle = _freeHandle;
                _freeHandle = _nextHandle[handle];
            }
            else
            {
                handle = _maxHandle++;
                if (handle >= _blocks)
                {
                    return false;
                }
            }

            int index = _count++;

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

        public void Set(int handle, T[] values)
        {
            _vertices.Sub(values, _blockSize * _handleToIndex[handle], _blockSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SwapAndPop(int indexToRemove)
        {
            int lastIndex = _count - 1;

            if (indexToRemove < lastIndex)
            {
                _vertices.Copy(_blockSize * lastIndex, _blockSize * indexToRemove, _blockSize);

                int movingHandle = _indexToHandle[lastIndex];
                _handleToIndex[movingHandle] = indexToRemove;
                _indexToHandle[indexToRemove] = movingHandle;
            }

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
