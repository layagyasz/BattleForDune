namespace BattleForDune.Core.Prototypes
{
    public record struct Progression(float Base, float PerLevel, float Cap)
    {
        public readonly float Materialize(byte level)
        {
            return Math.Min(Cap, Base + level * PerLevel);
        }
    }
}
