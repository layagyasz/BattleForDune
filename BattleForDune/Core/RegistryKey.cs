namespace BattleForDune.Core
{
    public readonly record struct RegistryKey(ushort Value)
    {
        public static readonly RegistryKey None = new();
    }
}
