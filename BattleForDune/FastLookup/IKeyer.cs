namespace BattleForDune.FastLookup
{
    public interface IKeyer<T>
    {
        int GetKey(in T value);
    }
}
