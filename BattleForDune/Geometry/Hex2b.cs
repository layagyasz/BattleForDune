using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace BattleForDune.Geometry
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Hex2b(sbyte q, sbyte r)
    {
        public readonly sbyte Q = q;
        public readonly sbyte R = r;
        public readonly sbyte S => (sbyte)(-Q - R);

        public Hex2b(int q, int r) : this((sbyte)q, (sbyte)r) { }

        public override bool Equals([NotNullWhen(true)] object? @object)
        {
            if (@object is null)
            {
                return false;
            }
            if (@object is Hex2b other)
            {
                return this == other;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Q, R);
        }

        public static bool operator ==(Hex2b left, Hex2b right)
        {
            return left.Q == right.Q && left.R == right.R;
        }

        public static bool operator !=(Hex2b left, Hex2b right)
        {
            return left.Q != right.Q || left.R != right.R;
        }

        public static Hex2b operator +(Hex2b left, Hex2b right)
        {
            return new((sbyte)(left.Q + right.Q), (sbyte)(left.R + right.R));
        }

        public static Hex2b operator -(Hex2b left, Hex2b right)
        {
            return new((sbyte)(left.Q - right.Q), (sbyte)(left.R - right.R));
        }

        public override string ToString()
        {
            return $"({Q}, {R}, {S})";
        }
    }
}
