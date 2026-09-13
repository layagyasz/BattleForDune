using BattleForDune.Core.Components.Effects.Types;

namespace BattleForDune.Core.Components.Effects
{
    public static class EffectFactory
    {
        public static IGenericEffect Materialize(EffectParameters @params) => @params.EffectType switch {
            EffectType.Burn => 
                new BurnEffect(
                    @params.InstanceId,
                    @params.TargetId,
                    @params.SubjectId,
                    @params.AbilityId,
                    @params.Stacks,
                    @params.Power),
            _ => throw new ArgumentException($"Unsupported event type {(int)@params.EffectType}")
        };
    }
}
