using System.Runtime.CompilerServices;

namespace BattleForDune.Geometry
{
    public readonly struct HexRect2b(Hex2b topLeft, Hex2b size)
    {
        public readonly Hex2b TopLeft = topLeft;
        public readonly Hex2b Size = size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Hex2b point)
        {
            return point.Q >= TopLeft.Q && point.Q < TopLeft.Q + Size.Q 
                && point.R >= TopLeft.R && point.R < TopLeft.R + Size.R;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(Hex2b position, int radius)
        {
            var closestQ = Math.Max(TopLeft.Q, Math.Min(position.Q, TopLeft.Q + Size.Q));
            var closestR = Math.Max(TopLeft.R, Math.Min(position.R, TopLeft.R + Size.R));
            return Hexes.Distance(position, new(closestQ, closestR)) <= radius;
        }
    }
}
