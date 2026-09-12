using BattleForDune.Core.Components.Abilities;

namespace BattleForDune.Core.Prototypes.Abilities
{
    public interface IHandlerPrototype
    {
        IGenericHandler Materialize();
    }
}
