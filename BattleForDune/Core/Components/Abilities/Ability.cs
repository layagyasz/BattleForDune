using BattleForDune.Core.Events;
using BattleForDune.Core.Schema;
using static BattleForDune.Core.Components.Abilities.IGenericAbilityHandler;

namespace BattleForDune.Core.Components.Abilities
{
    public record class Ability(
        RegistryKey AbilityKey, 
        EntityId OwnerId, 
        TargetType TargetType, 
        ITargetComparator TargetComparator, 
        IGenericAbilityHandler[] Handlers)
    {
        public class Prototype(
            RegistryKey AbilityKey,
            TargetType TargetType,
            ITargetComparator TargetComparator,
            IHandlerPrototype[] Handlers)
        {
            public Ability Materialize(EntityId OwnerId)
            {
                var handlers = new IGenericAbilityHandler[Handlers.Length];
                for (int i = 0; i < handlers.Length; ++i)
                {
                    handlers[i] = Handlers[i].Materialize();
                }
                return new Ability(AbilityKey, OwnerId, TargetType, TargetComparator, handlers);
            }
        }

        public void Intercept<T>(ref T @event, ComponentWorld world) where T : struct, IGameEvent
        {
            foreach (var handler in Handlers)
            {
                if (handler is IAbilityHandler<T> specificHandler)
                {
                    specificHandler.Intercept(ref @event, world);
                }
            }
        }

        public void Handle<T>(in T @event, IGameEventSink sink, ComponentWorld world)
            where T : struct, IGameEvent
        {
            foreach (var handler in Handlers)
            {
                if (handler is IAbilityHandler<T> specificHandler)
                {
                    specificHandler.Handle(in @event, world, sink);
                }
            }
        }
    }
}
