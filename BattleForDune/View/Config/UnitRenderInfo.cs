using BattleForDune.Core;

namespace BattleForDune.View.Config
{
    public readonly record struct UnitRenderInfo : IComparable<UnitRenderInfo>
    {
        public required RegistryKey Key { get; init; }
        public required string MarkerPath { get; init; }

        public readonly int CompareTo(UnitRenderInfo other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}
