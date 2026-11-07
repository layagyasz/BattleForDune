using BattleForDune.Core;
using OpenTK.Mathematics;

namespace BattleForDune.View.Config
{
    public readonly record struct AuraRenderInfo : IComparable<AuraRenderInfo>
    {
        public required RegistryKey Key { get; init; }
        public required Color4 Color { get; init; }
        public required RegistryKey[] AuraRenders { get; init; }

        public readonly int CompareTo(AuraRenderInfo other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}
