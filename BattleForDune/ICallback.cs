namespace BattleForDune
{
    public interface ICallback<T>
    {
        void Notify(in T value);
    }
}
