using MathNet.Numerics.RootFinding;
using OpenTK.Mathematics;

namespace BattleForDune.Geometry
{
    public class DenseHexGrid<T>(Vector2i size) where T : struct
    {
        public Vector2i Size => new(Width, Height);
        public int Width => _values.GetLength(0);
        public int Height => _values.GetLength(1);

        private readonly T[,] _values = new T[size.X, size.Y];

        public bool Contains(int x, int y)
        {
            return !(x < 0 || y < 0 || x >= Width || y >= Height);
        }

        public bool Contains(Vector2i offset)
        {
            return Contains(offset.X, offset.Y);
        }

        public bool Contains(Hex2b hex)
        {
            return Contains(Geometry.Projection.AxialToOffset(hex));
        }

        public T Get(int x, int y)
        {
            return Contains(x, y) ? _values[x, y] : default;
        }

        public IEnumerable<T> Values()
        {
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    yield return Get(i, j);
                }
            }
        }

        public T Get(Vector2i offset)
        {
            return Get(offset.X, offset.Y);
        }

        public T Get(Hex2b hex)
        {
            return Get(Geometry.Projection.AxialToOffset(hex));
        }

        public void Set(int x, int y, T value)
        {
            if (Contains(x, y))
            {
                _values[x, y] = value;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public void Set(Vector2i offset, T value)
        {
            Set(offset.X, offset.Y, value);
        }

        public void Set(Hex2b hex, T value)
        {
            Set(Geometry.Projection.AxialToOffset(hex), value);
        }
    }
}
