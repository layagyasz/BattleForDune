using BattleForDune.Core.Components.Effects;
using BattleForDune.Core.Events;
using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components
{
    public class EffectComponent(ConfigurationRegistry registry, int capacity)
    {
        private readonly ConfigurationRegistry _registry = registry;
        private readonly SparseStorage<Effect> _effects = new(capacity);
        private readonly UniqueIndex _byId = new(capacity);
        private readonly MultiIndex _byOwner = new(capacity, capacity);
        private readonly ComplexIndex _byType = new(Enum.GetValues<GameEventType>().Length, capacity, capacity);

        public bool Add(Effect effect)
        {
            if (_effects.Allocate(effect, out var handle))
            {
                _byId.Add(effect.Id.Value, handle);
                if (effect.OwnerId != EntityId.None)
                {
                    _byOwner.Add(effect.OwnerId.Value, handle);
                }
                if (effect.IsGlobal)
                {
                    foreach (var @event in _registry.GetEffectPrototype(effect.EffectKey).SupportedEvents)
                    {
                        _byType.Add((int)@event, handle);
                    }
                }
                return true;
            }
            return false;
        }

        public bool Remove<TCallback>(EntityId id, TCallback callback) 
            where TCallback : ICallback<Effect>, allows ref struct
        {
            if (_byId.RemoveOnly(id.Value, out var handle))
            {
                ref var effect = ref _effects.Get(handle);
                callback.Notify(handle, ref effect);
                if (effect.IsGlobal)
                {
                    foreach (var @event in _registry.GetEffectPrototype(effect.EffectKey).SupportedEvents)
                    {
                        _byType.Remove((int)@event, handle);
                    }
                }
                if (effect.OwnerId != EntityId.None)
                {
                    _byOwner.Remove(effect.OwnerId.Value, handle);
                }
                _effects.Free(handle);
                return true;
            }
            return false;
        }

        public ComponentEnumerator<SparseStorage<Effect>, Effect, MultiIndex.IndexEnumerator> 
            ByOwner(EntityId ownerId)
        {
            return new(_effects, _byOwner.GetEnumerator(ownerId.Value));
        }

        public ComponentEnumerator<SparseStorage<Effect>, Effect, ComplexIndex.IndexEnumerator> 
            ByEventType(GameEventType eventType)
        {
            return new(_effects, _byType.GetEnumerator((int)eventType));
        }
    }
}
