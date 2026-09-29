using BattleForDune.Core.Events;

namespace BattleForDune.Core.Components.Abilities
{
    internal interface IAbilityHandler<T> : IGenericAbilityHandler where T : struct, IGameEvent
    {
        public void Intercept(ref T @event, ComponentWorld world);

        public void Handle<TSink>(in T @event, ComponentWorld world, in TSink sink) 
            where TSink : IGameEventSink;
    }
}
