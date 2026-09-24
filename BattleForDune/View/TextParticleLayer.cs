using BattleForDune.Geometry;
using Cardamom;
using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BattleForDune.View
{
    public class TextParticleLayer(Font font, RenderShader shader, int capacity) : ManagedResource, IRenderable
    {
        public record class Factory(Font Font, RenderShader Shader)
        {
            public TextParticleLayer Create(int capacity)
            {
                return new(Font, Shader, capacity);
            }
        }

        private static readonly TextParticleVertex3[] _buffer = new TextParticleVertex3[128];

        private const float _scale = 0.025f;
        private const uint _fontSize = 12;
        private const float _invLifetime = 0.001f;

        private readonly int _capacity = capacity;
        private readonly Font _font = font;
        private readonly RenderShader _shader = shader;
        private readonly VertexBuffer<TextParticleVertex3> _vertices = new(PrimitiveType.Triangles);

        private int _cursor;
        private int _time;

        public void Add(string @string, Hex2b position, Color4 color)
        {
            // TODO: random velocity and starting position
            int count = 
                BuildMesh(
                    _buffer, @string, color, Geometry.Projection.ToCartesian(position), new(0, 0, -2), _time);
            if (_cursor + count >= _capacity)
            {
                _cursor = 0;
            }
            _vertices.Sub(_buffer, _cursor, count);
            _cursor = _cursor + count;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _shader.SetInt32("global_time", _time);
            _shader.SetFloat("inv_lifetime", _invLifetime);
            target.Draw(
                _vertices,
                0,
                _vertices.Length,
                new(BlendMode.Alpha, _shader, _font.GetTexure(_fontSize)!));
        }

        public void Initialize()
        {
            _vertices.Allocate(_capacity);
        }

        public void ResizeContext(Vector3 context) { }

        public void Update(long delta)
        {
            _time += (int)delta;
        }

        protected override void DisposeImpl()
        {
            _vertices.Dispose();
        }

        private int BuildMesh(
            TextParticleVertex3[] buffer,
            string @string,
            Color4 color, 
            Vector3 position, 
            Vector3 velocity, 
            int time)
        {
            var cursor = position;
            var index = 0;
            foreach (var character in @string)
            {
                if (char.IsWhiteSpace(character))
                {
                    cursor.X += _font!.GetWhitespace(_fontSize);
                    continue;
                }

                var glyph = _font!.GetOrLoadGlyph(character, _fontSize);

                float top = cursor.Z + _scale * glyph.Bounds.Min.Y;
                float bottom = cursor.Z + _scale * _scale * glyph.Bounds.Max.Y;
                float left = cursor.X + _scale * glyph.Bounds.Min.X;
                float right = cursor.X + _scale * glyph.Bounds.Max.X;

                float texTop = glyph.TextureView.Min.Y;
                float texBottom = glyph.TextureView.Max.Y;
                float texLeft = glyph.TextureView.Min.X;
                float texRight = glyph.TextureView.Max.X;

                buffer[index] = new(new(left, 0, top), color, new(texLeft, texTop), velocity, time);
                buffer[index + 1] = new(new(right, 0, top), color, new(texRight, texTop), velocity, time);
                buffer[index + 2] = new(new(left, 0, bottom), color, new(texLeft, texBottom), velocity, time);
                buffer[index + 3] = new(new(right, 0, top), color, new(texRight, texTop), velocity, time);
                buffer[index + 4] = new(new(left, 0, bottom), color, new(texLeft, texBottom), velocity, time);
                buffer[index + 5] = new(new(right, 0, bottom), color, new(texRight, texBottom), velocity, time);

                cursor.X += _scale * glyph.Advance;
                index += 6;
            }
            return index;
        }
    }
}
