namespace BattleForDune.FastLookup
{
    public interface IStorage<T>
    {
        int Count { get; }
        bool Allocate(T value, out int handle);
        bool Free(int handle);
        void FreeAll<TPredicate, TCallback>(in TPredicate predicate, in TCallback callback)
            where TPredicate : struct, IPredicate<T>, allows ref struct
            where TCallback : struct, ICallback<T>, allows ref struct;
        ref T Get(int handle);
    }
}
