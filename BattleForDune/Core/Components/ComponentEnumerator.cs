using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components
{
    public ref struct ComponentEnumerator<TStorage, TComponent>(
        TStorage storage, MultiIndex.IndexEnumerator enumerator) where TStorage : IStorage<TComponent>
    {
        private readonly TStorage _storage = storage;
        private MultiIndex.IndexEnumerator _enumerator = enumerator;

        public readonly TComponent Current => _storage.Get(_enumerator.Current);

        public readonly ComponentEnumerator<TStorage, TComponent> GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }
    }
}
