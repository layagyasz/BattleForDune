using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components
{
    public class StatComponent(int capacity) : SimpleComponent<StatBlock>(capacity) { }
}
