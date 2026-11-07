using BattleForDune.Geometry;
using OpenTK.Mathematics;

namespace BattleForDune.Core.Schema
{
    public readonly record struct Tile
    {
        public struct Transformation
        {
            private const byte _reflectionMask = 4;
            private readonly byte _value;

            public Transformation(byte value)
            {
                _value = value;
            }

            public Transformation(bool reflect, byte rotate)
            {
                _value = rotate;
                if (reflect)
                {
                    _value |= _reflectionMask;
                }
            }

            public readonly bool IsReflected()
            {
                return (_value & _reflectionMask) != 0;
            }

            public readonly byte GetRotation()
            {
                return (byte)(_value & 0x7);
            }

            private static readonly int[] _reflection = [1, 0, 5, 4, 3, 2];
            public readonly Vector2 Apply(int index)
            {
                index = (index + GetRotation()) % 6;
                if (IsReflected())
                {
                    index = _reflection[index];
                }
                return Hexes.Corners[index].Xz;
            }
        }

        public RegistryKey TileKey { get; init; }
        public Transformation Transform { get; init; }

        public Tile(RegistryKey tileKey, Transformation transform)
        {
            TileKey = tileKey;
            Transform = transform;
        }
    }
}
