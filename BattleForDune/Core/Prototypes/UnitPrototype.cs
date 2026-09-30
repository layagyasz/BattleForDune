namespace BattleForDune.Core.Prototypes
{
    public record class UnitPrototype(
        RegistryKey UnitKey, 
        StatBlockPrototype Stats,
        RegistryKey[] Passives,
        RegistryKey[] Abilities) : IComparable<UnitPrototype>
    {
        public int CompareTo(UnitPrototype? other)
        {
            if (other == null)
            {
                return 1;
            }
            return UnitKey.CompareTo(other.UnitKey);
        }
    }
}
