namespace BattleForDune.Core.Components
{
    public class InitiativeComponent(int capacity)
    {
        private int _count;
        private readonly EntityId[] _ids = new EntityId[capacity];
        private readonly sbyte[] _invInitiative = new sbyte[capacity];

        public void Add(EntityId id, sbyte initiative)
        {
            _ids[_count] = id;
            _invInitiative[_count++] = (sbyte)-initiative;
        }

        public void Clear()
        {
            _count = 0;
        }

        public ReadOnlySpan<EntityId> AsSpan()
        {
            return new ReadOnlySpan<EntityId>(_ids, 0, _count);
        }

        public void Sort()
        {
            Array.Sort(_invInitiative, _ids, 0, _count);
        }
    }
}
