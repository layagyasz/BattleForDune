using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Effects;
using BattleForDune.Core.Prototypes;

namespace BattleForDune.Core
{
    public class ConfigurationRegistry(
        Ability.Prototype[] abilities,
        IGenericEffect.IEffectPrototype[] effects,
        Faction[] factions, 
        UnitPrototype[] unitPrototypes)
    {
        private readonly Ability.Prototype[] _abilities = abilities;
        private readonly IGenericEffect.IEffectPrototype[] _effects = effects;
        private readonly Faction[] _factions = factions;
        private readonly UnitPrototype[] _unitPrototypes = unitPrototypes;

        public Ability.Prototype GetAbilityPrototype(RegistryKey key)
        {
            return _abilities[key.Value];
        }

        public IGenericEffect.IEffectPrototype GetEffectPrototype(RegistryKey key)
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
