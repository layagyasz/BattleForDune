namespace BattleForDune.FastLookup
{
    public interface IIndex
    {
        void Add(int key, int handle);
        void RemoveAll(int key);
        void Remove(int key, int handle);
    }
}
