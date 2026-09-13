using BattleForDune.Core.Components.Effects;
using BattleForDune.Core.Events;
using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components
{
    public class EffectComponent
    {
        private readonly SparseStorage<IGenericEffect> _effects = new(1024);
        private readonly UniqueIndex _byId = new(1024);
        private readonly MultiIndex _byType = new(1024, Enum.GetValues<GameEventType>().Length);

        public bool Add(EffectParameters @params)
        {
            // Merge with existing effect if possible.
            var effect = EffectFactory.Materialize(@params);
            if (_effects.Allocate(effect, out var handle))
            {
                _byId.Add(effect.Id.Value, handle);
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
            if (_byId.RemoveOnly(id.Value, out var handle))
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

        public ComponentEnumerator<SparseStorage<IGenericEffect>, IGenericEffect> Enumerate(
            GameEventType eventType)
        {
            return new(_effects, _byType.GetEnumerator((int)eventType));
        }
    }
}
