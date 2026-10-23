using BattleForDune.Core.Events;

namespace BattleForDune.Core.Components.Effects
{
    public record class Effect(
        RegistryKey EffectKey, 
        EntityId Id, 
        EntityId OwnerId, 
        EntityId SourceId, 
        byte AbilityId,
        bool IsGlobal,
        IGenericEffectHandler[] Handlers) 
        : IGameEventHandler
    {
        public record class Prototype(
            RegistryKey EffectKey, 
            GameEventType[] SupportedEvents, 
            bool IsMergeable,
            bool IsGlobal,
            IGenericEffectHandler[] Handlers) : IComparable<Prototype>
        {
            public int CompareTo(Prototype? other)
            {
                if (other == null)
                {
                    return 1;
                }
                return EffectKey.CompareTo(other.EffectKey);
            }

            public Effect Materialize(EntityId id, EntityId ownerId, EntityId sourceId, byte abilityId)
            {
                return new(EffectKey, id, ownerId, sourceId, abilityId, IsGlobal, Handlers);
            }
        }

        public byte Stacks { get; private set; }
        public byte Power { get; private set; }

        public void Apply<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            foreach (var handler in Handlers)
            {
                handler.Apply(this, world, in sink);
            }
        }

        public void Remove<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            foreach (var handler in Handlers)
            {
                handler.Remove(this, world, in sink);
            }
        }

        public void MergeStacks(byte newStacks)
        {
            Stacks += newStacks;
        }

        public void MergePower(byte newPower)
        {
            Power = Math.Max(Power, newPower);
        }

        public void Intercept<T>(ref T @event, ComponentWorld world) where T : struct, IGameEvent
        {
            foreach (var handler in Handlers)
            {
                if (handler is IEffectHandler<T> specificHandler)
                {
                    specificHandler.Intercept(this, ref @event, world);
                }
            }
        }

        public void Handle<T, TSink>(in T @event, ComponentWorld world, in TSink sink)
            where T : struct, IGameEvent
            where TSink : IGameEventSink
        {
            foreach (var handler in Handlers)
            {
                if (handler is IEffectHandler<T> specificHandler)
                {
                    specificHandler.Handle(this, in @event, world, sink);
                }
            }
        }
    }
}
