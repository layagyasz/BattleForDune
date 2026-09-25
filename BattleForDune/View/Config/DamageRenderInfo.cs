using BattleForDune.Core.Schema;
using OpenTK.Mathematics;

namespace BattleForDune.View.Config
{
    public record struct DamageRenderInfo : IComparable<DamageRenderInfo>
    {
        public required DamageType DamageType { get; set; }
        public required Color4 Color { get; set; }

        public readonly int CompareTo(DamageRenderInfo other)
        {
            return DamageType.CompareTo(other.DamageType);
        }
    }
}
