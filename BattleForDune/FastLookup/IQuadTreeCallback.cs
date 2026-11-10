using BattleForDune.Geometry;

namespace BattleForDune.FastLookup
{
    public interface IQuadTreeCallback<T>
    {
        void Notify(Hex2b position, T item);
    }
}
