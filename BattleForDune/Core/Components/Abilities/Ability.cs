using static BattleForDune.Core.Components.Abilities.IGenericHandler;

namespace BattleForDune.Core.Components.Abilities
{
    public record class Ability(
        RegistryKey AbilityKey, EntityId Id, EntityId OwnerId, IGenericHandler[] Handlers)
    {
        public class Prototype(RegistryKey AbilityKey, IHandlerPrototype[] Handlers)
        {
            public Ability Materialize(EntityId Id, EntityId OwnerId)
            {
                var handlers = new IGenericHandler[Handlers.Length];
                for (int i = 0; i < handlers.Length; ++i)
                {
                    handlers[i] = Handlers[i].Materialize();
                }
                return new Ability(AbilityKey, Id, OwnerId, handlers);
            }
        }
    }
}
