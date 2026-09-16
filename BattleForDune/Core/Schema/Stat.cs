using System.Runtime.InteropServices;

namespace BattleForDune.Core.Schema
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct Stat(sbyte Value, sbyte Temporary, sbyte Max)
    {
        public void Combine(in Stat other)
        {
            Value += other.Value;
            Value += other.Temporary;
            Temporary += other.Temporary;
            Max += other.Max;
            Value = Math.Max(Value, Max);
        }

        public void Remove(in Stat other)
        {
            var temp = Temporary;
            if (Temporary < other.Temporary)
            {
                Temporary = Math.Min(Temporary, (sbyte)0);
            }
            else
            {
                Temporary = (sbyte)Math.Max(0, Temporary - other.Temporary);
            }
            Max -= other.Max;
            Value = (sbyte)Math.Max(Value - other.Value - temp + Temporary, Max);
        }
    }
}
