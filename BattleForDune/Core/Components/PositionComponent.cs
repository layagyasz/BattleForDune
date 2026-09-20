using BattleForDune.FastLookup;
using BattleForDune.Geometry;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;

namespace BattleForDune.Core.Components
{
    public class PositionComponent(int capacity) : SimpleComponent<Hex2b>(capacity) { }
}
