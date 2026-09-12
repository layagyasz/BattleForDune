using BattleForDune.Core.Events.Types;
using System.Threading.Channels;

namespace BattleForDune.Core.Events
{
    public class EventPump<TProcessor> : IGameEventSink where TProcessor : IGameEventProcessor
    {
        private readonly TProcessor _processor;
        private readonly Channel<GameEvent> _channel;

        public EventPump(TProcessor processor)
        {
            _processor = processor;
            var options = 
                new BoundedChannelOptions(capacity: 1024)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleWriter = true,
                    SingleReader = true
                };
            _channel = Channel.CreateBounded<GameEvent>(options);
        }

        public void Queue<T>(in T payload) where T : struct, IGameEvent
        {
            TryQueue(in payload);
        }

        public bool TryQueue<T>(in T payload) where T : struct, IGameEvent
        {
            GameEvent wrapper = GameEvent.Create(payload);
            return _channel.Writer.TryWrite(wrapper);
        }

        public void Flush()
        {
            while (_channel.Reader.TryRead(out GameEvent @event))
            {
                _processor.Process(ref @event, this);
            }
        }
    }
}
