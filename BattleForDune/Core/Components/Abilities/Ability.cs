using BattleForDune.Core.Schema;
using static BattleForDune.Core.Components.Abilities.IGenericHandler;

namespace BattleForDune.Core.Components.Abilities
{
    public record class Ability(
        RegistryKey AbilityKey, 
        EntityId OwnerId, 
        TargetType TargetType, 
        ITargetComparator TargetComparator, 
        IGenericHandler[] Handlers)
    {
        public class Prototype(
            RegistryKey AbilityKey,
            TargetType TargetType,
            ITargetComparator TargetComparator,
            IHandlerPrototype[] Handlers)
        {
            public Ability Materialize(EntityId OwnerId)
            {
                var handlers = new IGenericHandler[Handlers.Length];
                for (int i = 0; i < handlers.Length; ++i)
                {
                    handlers[i] = Handlers[i].Materialize();
                }
                return new Ability(AbilityKey, OwnerId, TargetType, TargetComparator, handlers);
            }
        }
    }
}
