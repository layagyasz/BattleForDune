using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities
{
    public interface ITargetComparator
    {
        public int Compare(ComponentWorld world, EntityId subject, EntityId left, EntityId right);

        public class DefaultComparator : ITargetComparator
        {
            public static readonly DefaultComparator Instance = new();

            public int Compare(ComponentWorld world, EntityId subject, EntityId left, EntityId right)
            {
                // TODO: Compare position.

                ref var leftStats = ref world.GetStats().TryGet(left, out bool isFound);
                if (!isFound)
                {
                    return -1;
                }
                ref var rightStats = ref world.GetStats().TryGet(right, out isFound);
                if (!isFound)
                {
                    return 1;
                }

                sbyte leftThreat = leftStats[StatType.Threat].Total;
                sbyte rightThreat = rightStats[StatType.Threat].Total;
                if (leftThreat < rightThreat)
                {
                    return -1;
                }
                if (leftThreat > rightThreat)
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}
