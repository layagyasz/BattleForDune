namespace BattleForDune.Core.Schema
{
    [Flags]
    public enum ComparisonType : byte
    {
        None = 0,
        LT = 1,
        GT = 2,
        EQ = 4
    }
}
