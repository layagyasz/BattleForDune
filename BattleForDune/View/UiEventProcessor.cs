using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Geometry;
using BattleForDune.View.Config;

namespace BattleForDune.View
{
    public class UiEventProcessor(ViewConfigurationRegistry registry, ComponentWorld world) 
        : IGameEventProcessor
    {
        private readonly ViewConfigurationRegistry _registry = registry;
        private readonly ComponentWorld _world = world;

        private UnitLayer? _unitLayer;
        private TextParticleLayer? _textParticleLayer;

        public void AttachUnitLayer(UnitLayer? layer)
        {
            _unitLayer = layer;
        }

        public void AttachTextParticleLayer(TextParticleLayer? textParticleLayer)
        {
            _textParticleLayer = textParticleLayer;
        }

        public void Process<TSink>(ref GameEvent @event, TSink sink) where TSink : IGameEventSink
        {
            // TODO: break into individual methods
            // TODO: localize
            Hex2b pos;
            switch (@event.Type)
            {
                case GameEventType.AddEffect:
                    var effect = @event.ReadPayload<AddEffectEvent>();
                    var effectView = _registry.GetEffect(effect.EffectKey);
                    pos = _world.Positions.TryGet(effect.TargetId, out var _);
                    _textParticleLayer?.Add(effectView.Name, pos, effectView.Color);
                    return;
                case GameEventType.AddUnit:
                    var unit = @event.ReadPayload<AddUnitEvent>();
                    _unitLayer?.Add(unit.UnitId, unit.UnitPrototypeKey, unit.Position);
                    return;
                case GameEventType.Break:
                    var @break = @event.ReadPayload<BreakEvent>();
                    pos = _world.Positions.TryGet(@break.TargetId, out var _);
                    var render = _registry.GetEvent(@event.Type);
                    _textParticleLayer?.Add(render.Name, pos, render.Color);
                    return;
                case GameEventType.Damage:
                    var damage = @event.ReadPayload<DamageEvent>();
                    var damageView = _registry.GetDamage(damage.DamageType);
                    pos = _world.Positions.TryGet(damage.TargetId, out var isFound);
                    if (isFound)
                    {
                        _textParticleLayer?.Add(damage.Damage.ToString(), pos, damageView.Color);
                    }
                    return;
                case GameEventType.Kill:
                    var kill = @event.ReadPayload<KillEvent>();
                    _unitLayer?.Remove(kill.TargetId);
                    return;
                case GameEventType.Move:
                    var move = @event.ReadPayload<MoveEvent>();
                    _unitLayer?.Move(move.SubjectId, move.Destination);
                    return;
            }
        }
    }
}
