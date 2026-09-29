using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components
{
    public ref struct ComponentEnumerator<TStorage, TComponent, TEnumerator>(
        TStorage storage, TEnumerator enumerator) 
        where TStorage : IStorage<TComponent>
        where TEnumerator : IEnumerator<int>, allows ref struct
    {
        private readonly TStorage _storage = storage;
        private TEnumerator _enumerator = enumerator;

        public readonly TComponent Current => _storage.Get(_enumerator.Current);

        public readonly ComponentEnumerator<TStorage, TComponent, TEnumerator> GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }
    }
}
