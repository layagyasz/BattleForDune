using BattleForDune.Core.Schema;
using System.Text.Json.Serialization;

namespace BattleForDune.Core.Components.Abilities
{
    [JsonDerivedType(typeof(DefaultComparator), "Default")]
    public interface ITargetComparator
    {
        public int Compare(ComponentWorld world, EntityId subject, EntityId left, EntityId right);

        public record class DefaultComparator : ITargetComparator
        {
            public static readonly DefaultComparator Instance = new();

            public int Compare(ComponentWorld world, EntityId subject, EntityId left, EntityId right)
            {
                // TODO: Compare position.

                ref var leftStats = ref world.Stats.TryGet(left, out bool isFound);
                if (!isFound)
                {
                    return -1;
                }
                ref var rightStats = ref world.Stats.TryGet(right, out isFound);
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
