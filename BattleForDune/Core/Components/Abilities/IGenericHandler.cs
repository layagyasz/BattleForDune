namespace BattleForDune.Core.Components.Abilities
{
    public interface IGenericHandler
    {
        public interface IHandlerPrototype
        {
            IGenericHandler Materialize();
        }
    }
}
