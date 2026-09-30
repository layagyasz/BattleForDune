using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using static BattleForDune.Core.Components.Abilities.IGenericAbilityHandler;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class DamageCriticalMultiplierHandler(float Multiplier) : IAbilityHandler<DamageEvent>
    {
        public record class Prototype(float Multiplier) : IHandlerPrototype
        {
            public IGenericAbilityHandler Materialize()
            {
                return new DamageCriticalMultiplierHandler(Multiplier);
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
