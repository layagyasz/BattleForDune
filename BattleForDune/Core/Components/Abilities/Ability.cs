using BattleForDune.Core.Events;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities
{
    public record class Ability(
        EntityId Id,
        EntityId OwnerId,
        sbyte Cooldown,
        sbyte Range,
        TargetType TargetType, 
        ITargetComparator TargetComparator, 
        IGenericAbilityHandler[] Handlers)
    {
        public record class Prototype(
            RegistryKey AbilityKey,
            sbyte Cooldown,
            sbyte Charges,
            sbyte Range,
            TargetType TargetType,
            ITargetComparator TargetComparator,
            IGenericAbilityHandler[] Handlers) : IComparable<Prototype>
        {
            public int CompareTo(Prototype? other)
            {
                if (other == null)
                {
                    return 1;
                }
                return AbilityKey.CompareTo(other.AbilityKey);
            }

            public Ability Materialize(EntityId id, EntityId ownerId)
            {
                return new Ability(id, ownerId, Cooldown, Range, TargetType, TargetComparator, Handlers)
                {
                    IsOnCooldown = false,
                    Charges = Charges
                };
            }
        }

        public bool IsOnCooldown { get; set; }
        public sbyte Charges { get; private set; }

        public void Consume()
        {
            if (Charges > 0)
            {
                Charges--;
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
