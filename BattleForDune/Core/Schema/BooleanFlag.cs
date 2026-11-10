namespace BattleForDune.Core.Schema
{
    [Flags]
    public enum BooleanFlag : byte
    {
        None = 0,
        False = 1,
        True = 2,
        Any = 3
    }
}
