using BattleForDune.Core.Events;

namespace BattleForDune.Core.Components.Effects
{
    public interface IEffectHandler<T> : IGenericEffectHandler where T : struct, IGameEvent
    {
        void Intercept(Effect effect, ref T @event, ComponentWorld world);
        void Handle<TSink>(Effect parent, in T @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink;
    }
}
