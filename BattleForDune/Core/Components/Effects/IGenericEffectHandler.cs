using BattleForDune.Core.Events;

namespace BattleForDune.Core.Components.Effects
{
    public interface IGenericEffectHandler
    {
        void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink) where TSink : IGameEventSink;
        void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink) where TSink : IGameEventSink;

        public interface IHandlerPrototype
        {
            IGenericEffectHandler Materialize();
        }
    }
}
