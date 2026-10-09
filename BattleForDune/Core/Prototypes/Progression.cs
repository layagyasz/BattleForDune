using Cardamom.Mathematics;

namespace BattleForDune.Core.Prototypes
{
    public record struct Progression(float Base, float PerLevel, Interval Interval)
    {
        public readonly float Materialize(byte level)
        {
            return Interval.Clamp(Base + level * PerLevel);
        }
    }
}
