using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Abilities.Damage;

namespace BattleForDune.Core.Prototypes.Abilities.Damage
{
    public record class CriticalMultiplierPrototype(float Multiplier) : IHandlerPrototype
    {
        public IGenericHandler Materialize()
        {
            return new CriticalMultiplierHandler(Multiplier);
        }
    }
}
