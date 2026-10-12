namespace BattleForDune.Core.Prototypes
{
    public record struct Archetype(RegistryKey ArchetypeKey, StatBlockPrototype Stats) : IComparable<Archetype>
    {
        public readonly int CompareTo(Archetype other)
        {
            return ArchetypeKey.CompareTo(other.ArchetypeKey);
        }
    }
}
