using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Geometry;
using OpenTK.Mathematics;

namespace BattleForDune.View
{
    public class UiEventProcessor(ComponentWorld world) : IGameEventProcessor
    {
        private ComponentWorld _world = world;

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
            Hex2b pos;
            switch (@event.Type)
            {
                case GameEventType.AddEffect:
                    var effect = @event.ReadPayload<AddEffectEvent>();
                    pos = _world.GetPositions().TryGet(effect.TargetId, out var _);
                    // TODO: localize and color per damage type
                    _textParticleLayer?.Add(effect.GetType().Name, pos, Color4.Magenta);
                    return;
                case GameEventType.AddUnit:
                    var unit = @event.ReadPayload<AddUnitEvent>();
                    _unitLayer?.Add(unit.UnitId, unit.UnitPrototypeKey, unit.Position);
                    return;
                case GameEventType.Damage:
                    var damage = @event.ReadPayload<DamageEvent>();
                    pos = _world.GetPositions().TryGet(damage.TargetId, out var _);
                    _textParticleLayer?.Add(damage.Damage.ToString(), pos, Color4.Red);
                    return;
            }
        }
    }
}
