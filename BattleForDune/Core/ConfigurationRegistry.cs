using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Prototypes;

namespace BattleForDune.Core
{
    public class ConfigurationRegistry(
        Faction[] factions, Ability.Prototype[] abilities, UnitPrototype[] unitPrototypes)
    {
        private readonly Faction[] _factions = factions;
        private readonly Ability.Prototype[] _abilities = abilities;
        private readonly UnitPrototype[] _unitPrototypes = unitPrototypes;

        public Ability.Prototype GetAbilityPrototype(RegistryKey key)
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
