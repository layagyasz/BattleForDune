using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Systems;
using BattleForDune.Geometry;

namespace BattleForDune.Core
{
    public class LocalWorld(
        ComponentWorld components,
        EventPump<LocalEventProcessor> eventPump,
        int delay,
        Random random)
    {
        private readonly ComponentWorld _components = components;
        private readonly SystemWorld _systems = new(delay, random);
        private readonly EventPump<LocalEventProcessor> _events = eventPump;

        public void AddUnit(
            RegistryKey prototypeKey, byte level, RegistryKey factionKey, byte teamId, Hex2b position)
        {
            _events.Queue(new AddUnitEvent(prototypeKey, level, factionKey, teamId, position));
            _events.Flush();
        }

        public void Step()
        {
            _systems.Run(_components, _events);
        }
    }
}
