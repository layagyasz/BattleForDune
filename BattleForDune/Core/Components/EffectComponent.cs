using BattleForDune.Core.Components.Effects;
using BattleForDune.Core.Events;
using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components
{
    public class EffectComponent : IGameEventRouter
    {
        private readonly SparseStorage<IGenericEffect> _effects = new(1024);
        private readonly UniqueIndex _index = new(1024);
        private readonly MultiIndex _byType = new(1024, Enum.GetValues<GameEventType>().Length);

        public bool Add(IGenericEffect effect)
        {
            if (_effects.Allocate(effect, out var handle))
            {
                _index.Add(effect.Id.Value, handle);
                foreach (var @event in effect.SupportedEvents)
                {
                    _byType.Add((int)@event, handle);
                }
                return true;
            }
            return false;
        }

        public bool Remove(EntityId id)
        {
            if (_index.RemoveOnly(id.Value, out var handle))
            {
                ref var effect = ref _effects.Get(handle);
                foreach (var @event in effect.SupportedEvents)
                {
                    _byType.Remove((int)@event, handle);
                }
                _effects.Free(handle);
                return true;
            }
            return false;
        }

        public void Intercept<T>(ref T @event) where T : struct, IGameEvent
        {
            foreach (var handle in _byType.GetEnumerator((int)@event.Type))
            {
                ((IEffect<T>)_effects.Get(handle)).Intercept(ref @event);
            }
        }

        public void Handle<T>(in T @event, IGameEventSink sink) where T : struct, IGameEvent
        {
            foreach (var handle in _byType.GetEnumerator((int)@event.Type))
            {
                ((IEffect<T>)_effects.Get(handle)).Handle(in @event, sink);
            }
        }
    }
}
