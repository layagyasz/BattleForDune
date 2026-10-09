namespace BattleForDune.Core.Schema
{
    public static class BooleanFlags
    {
        public static bool HasFlag(this BooleanFlag flag, bool value)
        {
            return flag.HasFlag(value ? BooleanFlag.True : BooleanFlag.False);
        }
    }
}
