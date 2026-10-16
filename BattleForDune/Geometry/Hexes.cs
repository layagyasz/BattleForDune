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
        public static int Displacement(Hex2b delta)
        {
            return (Math.Abs(delta.Q) + Math.Abs(delta.R) + Math.Abs(delta.S)) >> 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Distance(Hex2b left, Hex2b right)
        {
            return Displacement(left - right);
        }

        public static Hex2b Lerp(Hex2b left, Hex2b right, float blend)
        {
            var l = new Hex2(left.Q, left.R);
            var r = new Hex2(right.Q, right.R);
            return SnapToHex(l + blend * (r - l));
        }

        public static Hex2b SnapToHex(Hex2 hex)
        {
            var q = (int)Math.Round(hex.Q);
            var r = (int)Math.Round(hex.R);
            var s = (int)Math.Round(hex.S);

            var dQ = Math.Abs(q - hex.Q);
            var dR = Math.Abs(r - hex.R);
            var dS = Math.Abs(s - hex.S);

            if (dQ > dR && dQ > dS)
            {
                q = -r - s;
            }
            else if (dR > dS)
            {
                r = -q - s;
            }
            return new(q, r);
        }
    }
}
