using OpenTK.Mathematics;

namespace BattleForDune.Geometry
{
    public static class Projection
    {
        private static readonly float _sqrt3 = MathF.Sqrt(3);

        public static Vector3 ToCartesian(Hex2b axial)
        {
            return new(1.5f * axial.Q, 0, 0.5f * _sqrt3 * axial.Q + _sqrt3 * axial.R);
        }
    }
}
