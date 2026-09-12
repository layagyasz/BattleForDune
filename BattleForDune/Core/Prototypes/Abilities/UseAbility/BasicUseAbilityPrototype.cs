using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Abilities.UseAbility;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Prototypes.Abilities.UseAbility
{
    public record class BasicUseAbilityPrototype : IHandlerPrototype
    {
        public IGenericHandler Materialize()
        {
            return new BasicUseAbilityHandler();
        }
    }
}
