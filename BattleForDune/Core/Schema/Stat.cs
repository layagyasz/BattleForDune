using System.Runtime.InteropServices;

namespace BattleForDune.Core.Schema
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct Stat(sbyte Value, sbyte Temporary, sbyte Max)
    {
        public static Stat Of(sbyte value)
        {
            return new(value, 0, value);
        }

        public void Add(in Stat other)
        {
            Value += other.Value;
            Temporary += other.Temporary;
            Max += other.Max;
        }

        public void Subtract(in Stat other)
        {
            if (Temporary < other.Temporary)
            {
                Temporary = Math.Min(Temporary, (sbyte)0);
            }
            else
            {
                Temporary = (sbyte)Math.Max(0, Temporary - other.Temporary);
            }
            Max -= other.Max;
            Value = (sbyte)Math.Max(Value - other.Value, Max);
        }
    }
}
