using System.Runtime.InteropServices;

namespace BattleForDune.Core.Schema
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct Stat(sbyte Value, sbyte Temporary, sbyte Max, sbyte Advantage)
    {
        public readonly sbyte Total => (sbyte)(Value + Temporary);

        public void Combine(in Stat other)
        {
            Value += other.Value;
            Temporary += other.Temporary;
            Max += other.Max;
            Advantage += other.Advantage;

            Value = Math.Max(Value, Max);
        }

        public void Remove(in Stat other)
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
            Advantage -= other.Advantage;

            Value = (sbyte)Math.Max(Value - other.Value, Max);
        }
    }
}
