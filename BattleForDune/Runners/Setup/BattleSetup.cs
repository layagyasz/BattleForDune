using BattleForDune.Core.Events;
using OpenTK.Mathematics;

namespace BattleForDune.Runners.Setup
{
    public record struct BattleSetup(byte TurnLimit, TeamSetup TeamA, TeamSetup TeamB)
    {
        public readonly void Setup<TSink>(Vector2i bounds, TSink sink) where TSink : IGameEventSink
        {
            TeamA.Setup(bounds, (byte)(bounds.Y / 2), -1, sink);
            TeamB.Setup(bounds, (byte)(bounds.Y / 2 + 1), 1, sink);
            sink.Flush();
        }
    }
}
