using BattleForDune.Core.Schema;
using OpenTK.Mathematics;

namespace BattleForDune.View.Config
{
    public readonly record struct DamageRenderInfo : IComparable<DamageRenderInfo>
    {
        public required DamageType DamageType { get; init; }
        public required Color4 Color { get; init; }

        public readonly int CompareTo(DamageRenderInfo other)
        {
            return DamageType.CompareTo(other.DamageType);
        }
    }
}
