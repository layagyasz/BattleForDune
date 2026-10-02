using System.Runtime.InteropServices;

namespace BattleForDune.Geometry
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Hex2(float q, float r)
    {
        public readonly float Q = q;
        public readonly float R = r;
        public readonly float S => (-Q - R);

        public static Hex2 operator +(Hex2 left, Hex2 right)
        {
            return new(left.Q + right.Q, left.R + right.R);
        }

        public static Hex2 operator -(Hex2 left, Hex2 right)
        {
            return new(left.Q - right.Q, left.R - right.R);
        }

        public static Hex2 operator *(Hex2 left, float right)
        {
            return new(right * left.Q, right * left.R);
        }

        public static Hex2 operator *(float left, Hex2 right)
        {
            return right * left;
        }

        public override string ToString()
        {
            return $"({Q}, {R}, {S})";
        }
    }
}
