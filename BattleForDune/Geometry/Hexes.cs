using OpenTK.Mathematics;
using System.Runtime.CompilerServices;

namespace BattleForDune.Geometry
{
    public static class Hexes
    {
        private static readonly float _sqrt3_2 = 0.5f * MathF.Sqrt(3);
        public static readonly Vector3[] Corners =
        [
            new(-0.5f, 0, -_sqrt3_2),
            new(0.5f, 0, -_sqrt3_2),
            new(1, 0, 0),
            new(0.5f, 0, _sqrt3_2),
            new(-0.5f, 0, _sqrt3_2),
            new(-1, 0, 0)
        ];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Displacement(Hex2b delta)
        {
            return (byte)(((byte)Math.Abs(delta.Q) + (byte)Math.Abs(delta.R) + (byte)Math.Abs(delta.S)) >> 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Distance(Hex2b left, Hex2b right)
        {
            return Displacement(left - right);
        }
    }
}
