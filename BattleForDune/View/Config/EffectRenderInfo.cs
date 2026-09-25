using BattleForDune.Core;
using OpenTK.Mathematics;

namespace BattleForDune.View.Config
{
    public record struct EffectRenderInfo : IComparable<EffectRenderInfo>
    {
        public required RegistryKey Key { get; set; }
        public required Color4 Color { get; set; }
        public required string Name { get; set; }

        public readonly int CompareTo(EffectRenderInfo other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}
