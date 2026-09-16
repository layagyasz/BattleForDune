using BattleForDune.Core.Components.Abilities;

namespace BattleForDune.Core.Prototypes.Abilities
{
    public class AbilityPrototype(RegistryKey Key, IHandlerPrototype[] Handlers)
    {
        public Ability Materialize(EntityId Id, EntityId OwnerId)
        {
            var handlers = new IGenericHandler[Handlers.Length];
            for (int i = 0; i < handlers.Length; ++i)
            {
                handlers[i] = Handlers[i].Materialize();
            }
            return new Ability(Id, OwnerId, handlers);
        }
    }
}
