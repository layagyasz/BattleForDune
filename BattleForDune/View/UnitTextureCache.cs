using BattleForDune.Core;
using BattleForDune.FastLookup;
using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BattleForDune.View
{
    public class UnitTextureCache(
        ViewConfigurationRegistry registry, Font font, RenderShader shader, int capacity)
    {
        private static Vector2i ElementSize = new(64, 96);
        private static Vector3 Scale = new(1f, 1f, 1f);
        private static readonly Vector3[] Vertices =
        {
            Scale * new Vector3(-1f, -1f, 0f),
            Scale * new Vector3(1f, -1f, 0f),
            Scale * new Vector3(-1f, 1f, 0f),

            Scale * new Vector3(-1f, 1f, 0f),
            Scale * new Vector3(1f, -1f, 0f),
            Scale * new Vector3(1f, 1f, 0f),
        };

        private static readonly float TextScale = 0.01f;
        private static readonly uint TextSize = 12;
        private static readonly Vector3 TextPin = new(0, 0.6f, 0);

        private readonly ViewConfigurationRegistry _registry = registry;
        private readonly Font _font = font;
        private readonly RenderShader _shader = shader;

        private readonly SparseStorage<Box2i> _segments = new(capacity);
        private readonly UniqueIndex _index = new(capacity);
        private readonly DynamicStaticSizeTexturePage _page =
            new(new(2048, 2048), ElementSize, new(1, 1, 1, 0), new(), new());

        public void AddAll(IEnumerable<RegistryKey> unitKeys)
        {
            using var renderTexture = new RenderTexture(ElementSize);
            renderTexture.PushProjection(
                new(-10, Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -10, 10)));
            renderTexture.PushViewMatrix(Matrix4.Identity);
            renderTexture.PushModelMatrix(Matrix4.Identity);
            using var text = new Text();
            text.SetFont(_font);
            text.SetColor(Color4.White);
            text.SetCharacterSize(TextSize);
            text.SetShader(_shader);

            foreach (var key in unitKeys)
            {
                RenderUnit(key, text, renderTexture);
                renderTexture.GetTexture().CopyToImage().SaveToFile("test.png");
                if (_page.Add(renderTexture.GetTexture(), out var box) 
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

        private static readonly Vertex3[] _vertices = new Vertex3[6];
        private void RenderUnit(RegistryKey key, Text text, RenderTexture target)
        {
            using Texture texture = Texture.FromFile(_registry.GetUnitTexture(key));
            AddSegment(
                _vertices,
                texture,
                Color4.White);
            target.Clear();
            target.Draw(
                _vertices,
                PrimitiveType.Triangles,
                0,
                6,
                new(BlendMode.Alpha, _shader, texture));

            var size = 0.01f * text.Size;

            // TODO: Render unit name
            /*
            target.PushModelMatrix(
                Matrix4.CreateScale(TextScale) *
                Matrix4.CreateTranslation(new(TextPin.X - 0.5f * size.X, TextPin.Y - size.Y, 0)));
            text.Draw(target, new SimpleUiContext());
            target.PopModelMatrix();
            */

            target.Display();
        }

        private static void AddSegment(Vertex3[] vertices, Texture texture, Color4 color)
        {
            vertices[0] = new(Vertices[0], color, new());
            vertices[1] = new(Vertices[1], color, new(texture.Size.X, 0));
            vertices[2] = new(Vertices[2], color, new(0, texture.Size.Y));
            vertices[3] = new(Vertices[3], color, new(0, texture.Size.Y));
            vertices[4] = new(Vertices[4], color, new(texture.Size.X, 0));
            vertices[5] = new(Vertices[5], color, texture.Size);
        }
    }
}
