using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using static BattleForDune.Core.Components.Abilities.IGenericHandler;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class CriticalMultiplierDamageHandler(float Multiplier) 
        : IGameEventHandler<DamageEvent>, IGenericHandler
    {
        public record class Prototype(float Multiplier) : IHandlerPrototype
        {
            public IGenericHandler Materialize()
            {
                return new CriticalMultiplierDamageHandler(Multiplier);
            }
        }

        public void Intercept(ref DamageEvent @event, ComponentWorld world)
        {
            if (!@event.IsCanceled && @event.IsCritical)
            {
                @event.Roll = (byte)(Multiplier * @event.Roll);
            }
        }

        public void Handle<TSink>(
            in DamageEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        { }
    }
}
