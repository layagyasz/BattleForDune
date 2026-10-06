namespace BattleForDune.Core.Schema
{
    [Flags]
    public enum DamageVector : byte
    {
        None = 0,
        Direct = 1,
        Indirect = 2,
        Evironmental = 4,
        All = 7
    }
}
