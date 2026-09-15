using BattleForDune.FastLookup;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;

namespace BattleForDune.Core.Components
{
    public class PositionComponent(int capacity) : SimpleComponent<Vector2i>(capacity) { }
}
