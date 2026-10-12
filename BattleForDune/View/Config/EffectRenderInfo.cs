using BattleForDune.Core;
using OpenTK.Mathematics;

namespace BattleForDune.View.Config
{
    public readonly record struct EffectRenderInfo : IComparable<EffectRenderInfo>
    {
        public required RegistryKey Key { get; init; }
        public required Color4 Color { get; init; }
        public required string Name { get; init; }
        public bool IsHidden { get; init; }

        public readonly int CompareTo(EffectRenderInfo other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}
