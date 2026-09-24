using BattleForDune.Core.Events;
using BattleForDune.View;

namespace BattleForDune.Controller
{
    public class BattleScreenController(
        EventPump<UiEventProcessor> eventPump, UiEventProcessor processor) : IUpdatingController
    {
        private readonly EventPump<UiEventProcessor> _eventPump = eventPump;
        private readonly UiEventProcessor _processor = processor;

        private BattleScreen? _screen;

        public void Bind(object @object)
        {
            _screen = (BattleScreen)@object;
            _processor.AttachUnitLayer(_screen.Scene.UnitLayer);
            _processor.AttachTextParticleLayer(_screen.Scene.TextParticleLayer);
        }
        
        public void Unbind()
        {
            _screen = null;
            _processor.AttachUnitLayer(null);
            _processor.AttachTextParticleLayer(null);
        }

        public void Update(long delta)
        {
            _eventPump.Flush();
        }
    }
}
