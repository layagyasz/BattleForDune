namespace BattleForDune.FastLookup
{
    public interface IPredicate<T>
    {
        bool Matches(in T value);
    }
}
