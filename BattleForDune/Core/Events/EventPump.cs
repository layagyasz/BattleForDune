using BattleForDune.Core.Events.Types;
using System.Threading.Channels;

namespace BattleForDune.Core.Events
{
    public class EventPump<TRouter, TProcessor> : IGameEventSink 
        where TRouter : IGameEventRouter
        where TProcessor : IGameEventProcessor
    {
        private readonly TRouter _handler;
        private readonly TProcessor _processor;
        private readonly Channel<GameEvent> _channel;

        public EventPump(TRouter handler, TProcessor processor)
        {
            _handler = handler;
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
                Process(ref @event);
            }
        }

        private void Process(ref GameEvent @event)
        {
            switch (@event.Type)
            {
                case GameEventType.Attack:
                    var attack = @event.ReadPayload<AttackEvent>();
                    _handler.Intercept(ref attack);
                    _processor.Process(in attack, this);
                    _handler.Handle(in attack, this);
                    return;
            }
        }
    }
}
