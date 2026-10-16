using BattleForDune.Core;
using BattleForDune.FastLookup;
using BattleForDune.View.Config;
using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using OpenTK.Mathematics;
namespace BattleForDune.View
{
    public class TileTextureCache(ViewConfigurationRegistry registry, int capacity)
    {
        private static Vector2i ElementSize = new(128, 128);

        private readonly ViewConfigurationRegistry _registry = registry;
        private readonly SparseStorage<Box2i> _segments = new(capacity);
        private readonly UniqueIndex _index = new(capacity);
        private readonly DynamicStaticSizeTexturePage _page =
            new(new(2048, 2048), ElementSize, new(1, 1, 1, 0), new(), new());

        public void AddAll(IEnumerable<RegistryKey> tileKeys)
        {
            foreach (var key in tileKeys)
            {
                using var texture = Texture.FromFile(_registry.GetTile(key).ImagePath);
                if (_page.Add(texture, out var box)
                    && _segments.Allocate(box, out var handle))
                {
                    _index.Add(key.Value, handle);
                }
            }
        }

        public Box2i Get(RegistryKey key)
        {
            if (_index.TryGet(key.Value, out var handle))
            {
                return _segments.Get(handle);
            }
            // TODO: Render on the fly or throw?
            return new();
        }

        public Texture GetTexture()
        {
            return _page.GetTexture();
        }
    }
}
