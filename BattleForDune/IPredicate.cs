namespace BattleForDune
{
    public interface IPredicate<T>
    {
        bool Matches(in T value);
    }
}
