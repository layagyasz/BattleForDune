using BattleForDune.Core.Prototypes;

namespace BattleForDune.Core
{
    public class ConfigurationRegistry(Faction[] factions, UnitPrototype[] unitPrototypes)
    {
        private readonly Faction[] _factions = factions;
        private readonly UnitPrototype[] _unitPrototypes = unitPrototypes;

        public Faction GetFaction(EntityId id)
        {
            return _factions[id.Value];
        }

        public UnitPrototype GetUnitPrototype(EntityId id)
        {
            return _unitPrototypes[id.Value];
        }
    }
}
