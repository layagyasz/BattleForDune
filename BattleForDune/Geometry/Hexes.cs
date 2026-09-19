using System.Runtime.CompilerServices;

namespace BattleForDune.Geometry
{
    public static class Hexes
    {
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
