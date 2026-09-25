using BattleForDune.Core;

namespace BattleForDune.View.Config
{
    public record struct UnitRenderInfo : IComparable<UnitRenderInfo>
    {
        public required RegistryKey Key { get; set; }
        public required string MarkerPath { get; set; }

        public readonly int CompareTo(UnitRenderInfo other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}
