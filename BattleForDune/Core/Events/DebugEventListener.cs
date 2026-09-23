namespace BattleForDune.Core.Events
{
    public class DebugEventListener : IGameEventSink
    {
        public void Queue<T>(in T @event) where T : struct, IGameEvent
        {
            Console.WriteLine(@event.Type);
        }

        public void Flush() { }
    }
}
