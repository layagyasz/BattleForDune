using BattleForDune.Core.Prototypes;
using BattleForDune.Core.Prototypes.Abilities;

namespace BattleForDune.Core
{
    public class ConfigurationRegistry(Faction[] factions, AbilityPrototype[] abilities, UnitPrototype[] unitPrototypes)
    {
        private readonly Faction[] _factions = factions;
        private readonly AbilityPrototype[] _abilities = abilities;
        private readonly UnitPrototype[] _unitPrototypes = unitPrototypes;

        public AbilityPrototype GetAbilityPrototype(RegistryKey key)
        {
            return _abilities[key.Value];
        }

        public Faction GetFaction(RegistryKey key)
        {
            return _factions[key.Value];
        }

        public UnitPrototype GetUnitPrototype(RegistryKey key)
        {
            return _unitPrototypes[key.Value];
        }
    }
}
