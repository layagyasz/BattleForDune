using BattleForDune.Core.Components.Timers;
using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components
{
    public class TimerComponent(int capacity) : IGameComponent<byte>
    {
        private struct TimerKeyer : IKeyer<GameTimer>
        {
            public readonly int GetKey(in GameTimer value)
            {
                return value.Turn;
            }
        }

        private readonly HeapStorage<TimerKeyer, GameTimer> _timers = new(new TimerKeyer(), capacity);
        private readonly UniqueIndex _index = new(capacity);

        public void Add(EntityId ownerId, byte turn)
        {
            if (_timers.Allocate(new(ownerId, turn), out var handle))
            {
                _index.Add(ownerId.Value, handle);
            }
        }

        public bool Peek(out GameTimer timer)
        {
            if (_timers.Count > 0)
            {
                timer = _timers.Peek();
                return true;
            }
            timer = default;
            return false;
        }

        public bool Remove(EntityId ownerId)
        {
            if (_index.RemoveOnly(ownerId.Value, out var handle))
            {
                _timers.Free(handle);
                return true;
            }
            return false;
        }

        public bool TryGet(EntityId ownerId, out byte expiration)
        { 
            if (_index.TryGet(ownerId.Value, out var handle))
            {
                expiration = _timers.Get(handle).Turn;
                return true;
            }
            expiration = 0;
            return false;
        }
    }
}
