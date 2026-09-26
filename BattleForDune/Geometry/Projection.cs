using OpenTK.Mathematics;

namespace BattleForDune.Geometry
{
    public static class Projection
    {
        private static readonly float _sqrt3 = MathF.Sqrt(3);

        public static Vector3 AxialToCartesian(Hex2b axial)
        {
            return new(1.5f * axial.Q, 0, 0.5f * _sqrt3 * axial.Q + _sqrt3 * axial.R);
        }

        public static Vector2i AxialToOffset(Hex2b axial)
        {
            int dir = axial.Q & 1;
            return new(axial.Q, axial.R + ((axial.Q - dir) >> 1));
        }

        public static Hex2b OffsetToAxial(Vector2i offset)
        {
            int dir = offset.X & 1;
            return new(offset.X, offset.Y - ((offset.X - dir) >> 1));
        }
    }
}
