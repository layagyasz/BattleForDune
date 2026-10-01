using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class DamageCriticalMultiplierHandler(float Multiplier) : IAbilityHandler<DamageEvent>
    {
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
