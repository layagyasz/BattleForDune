using Cardamom.Mathematics;

namespace BattleForDune.Core.Prototypes
{
    public record struct Progression(float Base, float PerLevel, Interval Interval)
    {
        public readonly float Materialize(byte level)
        {
            if (Interval.Minimum == 0 && Interval.Maximum == 0)
            {
                return Base + level * PerLevel;
            }
            return Interval.Clamp(Base + level * PerLevel);
        }
    }
}
