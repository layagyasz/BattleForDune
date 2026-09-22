using BattleForDune.Core;

namespace BattleForDune.View
{
    public class ViewConfigurationRegistry(string[] unitTextures)
    {
        private readonly string[] _unitTextures = unitTextures;

        public string GetUnitTexture(RegistryKey key)
        {
            return _unitTextures[key.Value];
        }
    }
}
