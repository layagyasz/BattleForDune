using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.View
{
    public class UiEventProcessor : IGameEventProcessor
    {
        private UnitLayer? _unitLayer;

        public void Attach(UnitLayer? layer)
        {
            _unitLayer = layer;
        }

        public void Process<TSink>(ref GameEvent @event, TSink sink) where TSink : IGameEventSink
        {
            switch (@event.Type)
            {
                case GameEventType.AddUnit:
                    var unit = @event.ReadPayload<AddUnitEvent>();
                    _unitLayer?.Add(unit.UnitId, unit.UnitPrototypeKey, unit.Position);
                    return;
            }
        }
    }
}
