using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using static BattleForDune.Core.Components.Abilities.IGenericHandler;

namespace BattleForDune.Core.Components.Abilities.Damage
{
    public record class CriticalMultiplierHandler(float Multiplier) 
        : IGameEventHandler<DamageEvent>, IGenericHandler
    {
        public record class Prototype(float Multiplier) : IHandlerPrototype
        {
            public IGenericHandler Materialize()
            {
                return new CriticalMultiplierHandler(Multiplier);
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
