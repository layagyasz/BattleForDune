namespace BattleForDune.Core
{
    public readonly record struct LocalizationKey(ushort Value)
    {
        public static readonly LocalizationKey None = new();
    }
}
