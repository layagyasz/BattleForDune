using BattleForDune.Core.Components;
using BattleForDune.Core.Events;

namespace BattleForDune.Core.Systems
{
    public interface IGameSystem
    {
        void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink;
    }
}
