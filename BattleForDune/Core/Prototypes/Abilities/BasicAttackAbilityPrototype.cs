using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Prototypes.Abilities
{
    public record class BasicAttackAbilityPrototype(IHandlerPrototype[] Handlers) 
        : IAbilityPrototype
    {
        public IAbility Materialize(EntityId Id, EntityId OwnerId)
        {
            var handlers = new IGenericHandler[Handlers.Length];
            for (int i = 0; i<handlers.Length;++i)
            {
                handlers[i] = Handlers[i].Materialize();
            }
            return new BasicAttackAbility(Id, OwnerId, handlers);
        }
    }
}
