using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Components.Abilities.Procs
{
    public record class Critical(float Multiplier) : IGameEventHandler<DamageEvent>
    {
        public void Intercept(ref DamageEvent @event, ComponentWorld world)
        {
            @event.Damage = (byte)(Multiplier * @event.Damage);
        }

        public void Handle<TSink>(
            in DamageEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        { }
    }
}
