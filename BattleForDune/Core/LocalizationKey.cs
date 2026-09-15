namespace BattleForDune.Core
{
    public readonly record struct LocalizationKey(int Value)
    {
        public static readonly LocalizationKey None = new(0);
    }
}
