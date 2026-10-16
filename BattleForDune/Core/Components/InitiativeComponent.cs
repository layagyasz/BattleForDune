namespace BattleForDune.Core.Components
{
    public class InitiativeComponent(int capacity)
    {
        private int _count;
        private readonly EntityId[] _ids = new EntityId[capacity];
        private readonly float[] _invInitiative = new float[capacity];

        public void Add(EntityId id, float initiative)
        {
            _ids[_count] = id;
            _invInitiative[_count++] = -initiative;
        }

        public void Clear()
        {
            Array.Clear(_ids, 0, _count);
            Array.Clear(_invInitiative, 0, _count);
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
