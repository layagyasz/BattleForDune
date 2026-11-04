using BattleForDune.Core;
using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.View.Config;
using System.Text;

namespace BattleForDune.View
{
    public class UiEventProcessor(ViewConfigurationRegistry registry, ComponentWorld world) 
        : IGameEventProcessor
    {
        private readonly ViewConfigurationRegistry _registry = registry;
        private readonly ComponentWorld _world = world;

        private AuraLayer? _auraLayer;
        private UnitLayer? _unitLayer;
        private TextParticleLayer? _textParticleLayer;

        public void AttachAuraLayer(AuraLayer? layer)
        {
            _auraLayer = layer;
        }

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
                case GameEventType.AddAura:
                    HandleAddAura(@event.ReadPayload<AddAuraEvent>());
                    return;
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
                case GameEventType.RemoveAura:
                    HandleRemoveAura(@event.ReadPayload<RemoveAuraEvent>());
                    return;
            }
        }

        private void HandleAddAura(AddAuraEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _auraLayer?.Add(@event.AuraId, @event.AuraKey, @event.Position, @event.Range);
        }

        private void HandleAddEffect(AddEffectEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            var effectView = _registry.GetEffect(@event.EffectKey);
            if (effectView.IsHidden)
            {
                return;
            }
            ref var pos = ref _world.Positions.TryGet(@event.TargetId, out var isFound);
            if (isFound)
            {
                _textParticleLayer?.Add(effectView.Name, pos, effectView.Color);
            }
        }

        private void HandleAddUnit(AddUnitEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            if (@event.SubjectId != EntityId.None)
            {
                var render = _registry.GetEvent(@event.Type);
                _textParticleLayer?.Add(render.Name, @event.Position, render.Color);
            }
                _unitLayer?.Add(@event.TargetId, @event.UnitPrototypeKey, @event.Position);
        }

        private void HandleBreak(BreakEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            ref var pos = ref _world.Positions.TryGet(@event.TargetId, out var isFound);
            if (isFound)
            {
                var render = _registry.GetEvent(@event.Type);
                _textParticleLayer?.Add(render.Name, pos, render.Color);
            }
        }

        private void HandleDamage(DamageEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            var damageView = _registry.GetDamage(@event.DamageType);
            ref var pos = ref _world.Positions.TryGet(@event.TargetId, out var isFound);
            if (isFound)
            {
                var builder = new StringBuilder();
                builder.Append(@event.Damage);
                if (@event.IsCritical)
                {
                    builder.Append('!');
                }
                _textParticleLayer?.Add(builder.ToString(), pos, damageView.Color);
            }
        }

        private void HandleKill(KillEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _unitLayer?.Remove(@event.TargetId);
        }

        private void HandleMove(MoveEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            ref var metadata = ref _world.UnitMetadatas.TryGet(@event.SubjectId, out var isFound);
            if (!isFound)
            {
                return;
            }
            _unitLayer?.Move(@event.SubjectId, metadata.UnitKey, @event.Destination);
        }

        private void HandleRemoveAura(RemoveAuraEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _auraLayer?.Remove(@event.AuraId);
        }
    }
}
