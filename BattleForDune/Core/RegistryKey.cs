using System.Collections.ObjectModel;

namespace BattleForDune.Core
{
    public readonly record struct RegistryKey(ushort Value) : IComparable<RegistryKey>
    {
        public static readonly RegistryKey None = new();

        public readonly int CompareTo(RegistryKey other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}
