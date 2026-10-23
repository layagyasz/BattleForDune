namespace BattleForDune.Core.Schema
{
    [Flags]
    public enum AbilityVector : byte
    {
        None = 0,
        Melee = 1,
        Ranged = 2,
        Gas = 4,
        Pathogen = 8,
        Poison = 16,
        Psychic = 32,
        Environmental = 64,
        All = 255
    }
}
