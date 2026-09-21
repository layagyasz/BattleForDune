namespace BattleForDune.Core.Schema
{
    [Flags]
    public enum TargetType : byte
    {
        None = 0,
        Self = 1,
        Friend = 2,
        Enemy = 4
    }
}
