namespace BattleForDune.Core.Prototypes
{
    public record class UnitPrototype(
        RegistryKey UnitKey, 
        StatBlockPrototype Stats,
        RegistryKey[] Abilities, 
        RegistryKey[] Passives)
    {
        public record class Description(
            RegistryKey UnitKey,
            RegistryKey Archetype,
            StatModifier[] Modifiers,
            RegistryKey[] Abilities,
            RegistryKey[] Passives) : IComparable<Description>
        {
            public int CompareTo(Description? other)
            {
                if (other == null)
                {
                    return 1;
                }
                return UnitKey.CompareTo(other.UnitKey);
            }

            public UnitPrototype Build(ConfigurationRegistry registry)
            {
                var stats = registry.GetArchetype(Archetype).Stats;
                foreach (var modifier in Modifiers)
                {
                    stats[modifier.Stat] = Combine(stats[modifier.Stat], modifier);
                }
                return new(UnitKey, stats, Abilities, Passives);
            }

            private static Progression Combine(Progression progression, StatModifier modifier)
            {
                return new(
                    progression.Base + modifier.Modifier, 
                    progression.PerLevel,
                    new(progression.Interval.Minimum, Math.Min(progression.Interval.Maximum, modifier.Cap)));
            }
        }
    }
}
