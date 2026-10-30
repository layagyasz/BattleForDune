using Cardamom.Logging;

namespace BattleForDune.Core.Events
{
    public class DebugEventListener(ILogger logger) : IGameEventSink
    {
        private readonly ILogger _logger = logger;

        public void Queue<T>(in T @event) where T : struct, IGameEvent
        {
            //_logger.AtInfo().Log(@event.ToString()!);
        }

        public void Flush() { }
    }
}
