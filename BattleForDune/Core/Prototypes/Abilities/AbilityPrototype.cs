using BattleForDune.Core.Components.Abilities;

namespace BattleForDune.Core.Prototypes.Abilities
{
    public class AbilityPrototype(IHandlerPrototype[] Handlers)
    {
        public IAbility Materialize(EntityId Id, EntityId OwnerId)
        {
            var handlers = new IGenericHandler[Handlers.Length];
            for (int i = 0; i < handlers.Length; ++i)
            {
                handlers[i] = Handlers[i].Materialize();
            }
            return new BasicAttackAbility(Id, OwnerId, handlers);
        }
    }
}
