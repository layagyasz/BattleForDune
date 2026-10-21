using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Systems;

namespace BattleForDune.Core
{
    public class LocalWorld(
        ComponentWorld components,
        EventPump<LocalEventProcessor> eventPump,
        byte turnLimit,
        int delay,
        Random random)
    {
        private readonly ComponentWorld _components = components;
        private readonly SystemWorld _systems = new(turnLimit, delay, random);
        private readonly EventPump<LocalEventProcessor> _events = eventPump;

        public void Resolve()
        {
            while (_components.State != ComponentWorld.WorldState.Terminated)
            {
                Step();
            }
        }

        public void Step()
        {
            _systems.Run(_components, _events);
        }
    }
}
