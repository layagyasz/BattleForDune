namespace BattleForDune.Core.Schema
{
    [Flags]
    public enum AbilityVector : byte
    {
        None = 0,
        Melee = 1,
        Ranged = 2,
        Gas = 4,
        Disease = 8,
        Psychic = 16,
        Environmental = 32,
        All = 63
    }
}
