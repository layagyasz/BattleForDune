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
            _processor.Attach(_screen.Scene.UnitLayer);
        }
        
        public void Unbind()
        {
            _screen = null;
            _processor.Attach(null);
        }

        public void Update(long delta)
        {
            _eventPump.Flush();
        }
    }
}
