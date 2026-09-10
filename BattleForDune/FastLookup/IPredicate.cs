namespace BattleForDune.FastLookup
{
    public interface IPredicate<T>
    {
        bool Matches(ref T value);
    }
}
