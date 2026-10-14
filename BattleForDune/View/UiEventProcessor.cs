using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
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
            // TODO: localize
            switch (@event.Type)
            {
                case GameEventType.AddEffect:
                    HandleAddEffect(@event.ReadPayload<AddEffectEvent>());
                    return;
                case GameEventType.AddUnit:
                    HandleAddUnit(@event.ReadPayload<AddUnitEvent>());
                    return;
                case GameEventType.Break:
                    HandleBreak(@event.ReadPayload<BreakEvent>());
                    return;
                case GameEventType.Damage:
                    HandleDamage(@event.ReadPayload<DamageEvent>());
                    return;
                case GameEventType.Kill:
                    HandleKill(@event.ReadPayload<KillEvent>());
                    return;
                case GameEventType.Move:
                    HandleMove(@event.ReadPayload<MoveEvent>());
                    return;
            }
        }

        private void HandleAddEffect(AddEffectEvent @event)
        {
            var effectView = _registry.GetEffect(@event.EffectKey);
            if (!effectView.IsHidden)
            {
                ref var pos = ref _world.Positions.TryGet(@event.TargetId, out var _);
                _textParticleLayer?.Add(effectView.Name, pos, effectView.Color);
            }
        }

        private void HandleAddUnit(AddUnitEvent @event)
        {
            _unitLayer?.Add(@event.UnitId, @event.UnitPrototypeKey, @event.Position);
        }

        private void HandleBreak(BreakEvent @event)
        {
            ref var pos = ref _world.Positions.TryGet(@event.TargetId, out var _);
            var render = _registry.GetEvent(@event.Type);
            _textParticleLayer?.Add(render.Name, pos, render.Color);
        }

        private void HandleDamage(DamageEvent @event)
        {
            var damageView = _registry.GetDamage(@event.DamageType);
            ref var pos = ref _world.Positions.TryGet(@event.TargetId, out var isFound);
            if (isFound)
            {
                _textParticleLayer?.Add(@event.Damage.ToString(), pos, damageView.Color);
            }
        }

        private void HandleKill(KillEvent @event)
        {
            _unitLayer?.Remove(@event.TargetId);
        }

        private void HandleMove(MoveEvent @event)
        {
            _unitLayer?.Move(@event.SubjectId, @event.Destination);
        }
    }
}
