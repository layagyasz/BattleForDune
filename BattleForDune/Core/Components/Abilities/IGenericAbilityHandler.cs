namespace BattleForDune.Core.Components.Abilities
{
    public interface IGenericAbilityHandler
    {
        public interface IHandlerPrototype
        {
            IGenericAbilityHandler Materialize();
        }
    }
}
