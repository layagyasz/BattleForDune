using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Systems;
using BattleForDune.Geometry;
using Cardamom.Utils.Suppliers.Ids;

namespace BattleForDune.Core
{
    public class LocalWorld
    {
        private readonly ComponentWorld _components;
        private readonly SystemWorld _systems;
        private readonly LocalEventProcessor _processor;
        private readonly EventPump<LocalEventProcessor> _events;

        public LocalWorld(ConfigurationRegistry registry, Random random)
        {
            _components = 
                new ComponentWorld(
                    registry, 
                    new LinearCongruentialIdSupplier(0x9E3779B1, (uint)(2 * random.Next() + 1), random.Next()),
                    random);
            _processor = new LocalEventProcessor(_components);
            _events = new EventPump<LocalEventProcessor>(_processor);
            _systems = new(random);
        }

        public void AddUnit(RegistryKey prototypeKey, RegistryKey factionKey, byte teamId, Hex2b position)
        {
            _events.Queue(new AddUnitEvent(prototypeKey, factionKey, teamId, position));
            _events.Flush();
        }

        public void Step()
        {
            _systems.Run(_components, _events);
        }
    }
}
