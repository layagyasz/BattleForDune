namespace BattleForDune.FastLookup
{
    public interface ICallback<T>
    {
        void Notify(int handle, ref T value);
    }
}
