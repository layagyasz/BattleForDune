using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Components.Effects
{
    public record class SimpleEffect(EntityId Id) : IEffect<AttackEvent>
    {
        private static readonly GameEventType[] _supportedEvents = [GameEventType.Attack];

        public ReadOnlySpan<GameEventType> SupportedEvents => _supportedEvents.AsSpan();

        public void Intercept(ref AttackEvent @event)
        {
            Console.WriteLine("intercepted!");
        }

        public void Handle(in AttackEvent @event, IGameEventSink sink)
        {
            Console.WriteLine("reacted!");
        }
    }
}
