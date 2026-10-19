using BattleForDune.Core.Components.Actions;

namespace BattleForDune.Core.Components
{
    public class ActionComponent(int capacity) : SimpleComponent<ActionPoints>(capacity);
}
