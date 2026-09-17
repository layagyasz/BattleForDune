namespace BattleForDune.Core.Components.Effects
{
    public record struct EffectParameters(
        EntityId SubjectId, 
        EntityId AbilityId, 
        EntityId TargetId,
        EntityId InstanceId,
        byte Stacks, 
        byte Power, 
        RegistryKey EffectKey);
}
