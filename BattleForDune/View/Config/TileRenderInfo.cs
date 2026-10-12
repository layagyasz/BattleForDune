using BattleForDune.Core;

namespace BattleForDune.View.Config
{
    public readonly struct TileRenderInfo : IComparable<TileRenderInfo>
    {
        public required RegistryKey Key { get; init; }
        public required string ImagePath { get; init; }

        public readonly int CompareTo(TileRenderInfo other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}
