using BattleForDune.Core.Prototypes;

namespace BattleForDune.Core.Components
{
    public class ConfigurationRegistry(UnitPrototype[] unitPrototypes)
    {
        private readonly UnitPrototype[] _unitPrototypes = unitPrototypes;

        public UnitPrototype GetUnitPrototype(EntityId id)
        {
            return _unitPrototypes[id.Value];
        }
    }
}
