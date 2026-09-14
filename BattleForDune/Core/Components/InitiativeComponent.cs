namespace BattleForDune.Core.Components
{
    public class InitiativeComponent
    {
        private int _count;
        private readonly EntityId[] _ids = new EntityId[1024];
        private readonly sbyte[] _invInitiative = new sbyte[1024];

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
