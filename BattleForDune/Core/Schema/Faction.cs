namespace BattleForDune.Core.Schema
{
    public record class Faction(RegistryKey FactionKey) : IComparable<Faction>
    {
        public int CompareTo(Faction? other)
        {
            if (other == null)
            {
                return 1;
            }
            return FactionKey.CompareTo(other.FactionKey);
        }
    }
}
