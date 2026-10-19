namespace BattleForDune.Core.Components.Actions
{
    public struct ActionPoints
    {
        public byte AttackPoints;
        public byte MovePoints;
        public byte AttacksLimit;
        public byte MovesLimit;

        public static ActionPoints Create()
        {
            return new()
            {
                AttacksLimit = 1,
                MovesLimit = 1
            };
        }
    }
}
