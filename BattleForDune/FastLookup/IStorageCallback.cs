namespace BattleForDune.FastLookup
{
    public interface IStorageCallback<T>
    {
        void Notify(int handle, ref T value);
    }
}
