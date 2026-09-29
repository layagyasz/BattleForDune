using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Effects;
using BattleForDune.Core.Prototypes;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core
{
    public class ConfigurationRegistry(
        Ability.Prototype[] abilities,
        Effect.Prototype[] effects,
        Faction[] factions, 
        UnitPrototype[] unitPrototypes)
    {
        private readonly Ability.Prototype[] _abilities = abilities;
        private readonly Effect.Prototype[] _effects = effects;
        private readonly Faction[] _factions = factions;
        private readonly UnitPrototype[] _unitPrototypes = unitPrototypes;

        public Ability.Prototype GetAbilityPrototype(RegistryKey key)
        {
            return _abilities[key.Value];
        }

        public Effect.Prototype GetEffectPrototype(RegistryKey key)
        {
            return _effects[key.Value];
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
