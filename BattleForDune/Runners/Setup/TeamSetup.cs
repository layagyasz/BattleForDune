using BattleForDune.Core;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using OpenTK.Mathematics;

namespace BattleForDune.Runners.Setup
{
    public record struct TeamSetup(
        byte TeamId, RegistryKey Faction, RegistryKey Unit, byte Level, int Count, int Width)
    {
        public readonly void Setup<TSink>(
            Vector2i bounds, byte front, sbyte direction, TSink sink) where TSink : IGameEventSink
        {
            for (int i = 0; i < Count; ++i)
            {
                var position = 
                    new Vector2i((bounds.X - Width) / 2 + (i % Width), front + direction * i / Width);
                sink.Queue(
                    new AddUnitEvent(
                        Unit, Level, Faction, TeamId, Geometry.Projection.OffsetToAxial(position)));
            }
        }
    }
}
