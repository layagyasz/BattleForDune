using BattleForDune.Core;

namespace BattleForDune.View.Config
{
    public struct TileRenderInfo : IComparable<TileRenderInfo>
    {
        public required RegistryKey Key { get; set; }
        public required string ImagePath { get; set; }

        public readonly int CompareTo(TileRenderInfo other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}
