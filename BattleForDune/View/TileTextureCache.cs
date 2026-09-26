using BattleForDune.Core;
using BattleForDune.FastLookup;
using BattleForDune.View.Config;
using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace BattleForDune.View
{
    public class TileTextureCache(ViewConfigurationRegistry registry, RenderShader shader, int capacity)
    {
        private static Vector2i ElementSize = new(128, 128);
        private static readonly Vertex3[] _vertices =
        [
            new Vertex3(new(-1f, -1f, 0f), Color4.White, new(0,0)),
            new Vertex3(new(1f, -1f, 0f), Color4.White, new(ElementSize.X, 0)),
            new Vertex3(new(-1f, 1f, 0f), Color4.White, new(0, ElementSize.Y)),

            new Vertex3(new(-1f, 1f, 0f), Color4.White, new(0, ElementSize.Y)),
            new Vertex3(new(1f, -1f, 0f), Color4.White, new(ElementSize.X, 0)),
            new Vertex3(new(1f, 1f, 0f), Color4.White, new(ElementSize.X, ElementSize.Y))
        ];

        private readonly ViewConfigurationRegistry _registry = registry;
        private readonly RenderShader _shader = shader;

        private readonly SparseStorage<Box2i> _segments = new(capacity);
        private readonly UniqueIndex _index = new(capacity);
        private readonly DynamicStaticSizeTexturePage _page =
            new(new(2048, 2048), ElementSize, new(1, 1, 1, 0), new(), new());

        public void AddAll(IEnumerable<RegistryKey> tileKeys)
        {
            using var renderTexture = new RenderTexture(ElementSize);
            renderTexture.PushProjection(
                new(-10, Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -10, 10)));
            renderTexture.PushViewMatrix(Matrix4.Identity);
            renderTexture.PushModelMatrix(Matrix4.Identity);

            foreach (var key in tileKeys)
            {
                RenderTile(key, renderTexture);
                if (_page.Add(renderTexture.GetTexture(), out var box)
                    && _segments.Allocate(box, out var handle))
                {
                    _index.Add(key.Value, handle);
                }
            }
            _page.GetTexture().CopyToImage().SaveToFile("tiles.png");
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

        private void RenderTile(RegistryKey key, RenderTexture target)
        {
            using Texture texture = Texture.FromFile(_registry.GetTile(key).ImagePath);
            target.Clear();
            target.Draw(
                _vertices,
                PrimitiveType.Triangles,
                0,
                18,
                new(BlendMode.Alpha, _shader, texture));
            target.Display();
        }
    }
}
