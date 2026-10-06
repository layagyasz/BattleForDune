using BattleForDune.Geometry;
using Cardamom;
using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BattleForDune.View
{
    public class TextParticleLayer(
        Font font, RenderShader shader, Random random, int capacity) : ManagedResource, IRenderable
    {
        public record class Factory(Font Font, RenderShader Shader)
        {
            public TextParticleLayer Create(int capacity)
            {
                return new(Font, Shader, new(), capacity);
            }
        }

        private static readonly TextParticleVertex3[] _buffer = new TextParticleVertex3[256];

        private const float _scale = 0.01f;
        private const uint _fontSize = 24;
        private const float _outline = 2f;
        private const float _invLifetime = 0.001f;
        private static readonly float _sqrt3_2 = 0.5f * MathF.Sqrt(3);


        private readonly int _capacity = capacity;
        private readonly Font _font = font;
        private readonly RenderShader _shader = shader;
        private readonly Random _random = random;
        private readonly VertexBuffer<TextParticleVertex3> _vertices = new(PrimitiveType.Triangles);

        private int _cursor;
        private int _time;

        public void Add(string @string, Hex2b position, Color4 color)
        {
            int count = 
                BuildMesh(
                    _buffer,
                    @string,
                    color, 
                    Geometry.Projection.AxialToCartesian(position) 
                        + new Vector3(_random.NextSingle() - 0.5f, 0f, -_sqrt3_2 * _random.NextSingle()),
                    new(0, 0, -2), 
                    _time);
            if (_cursor + count >= _capacity)
            {
                _cursor = 0;
            }
            _vertices.Sub(_buffer, _cursor, count);
            _cursor += count;
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

                var glyph = _font!.GetOrLoadGlyph(char.ToUpper(character), _fontSize);

                AddGlyph(buffer, index, glyph, _outline, Color4.Black, cursor, velocity, time);
                index += 6;
                AddGlyph(buffer, index, glyph, 0, color, cursor, velocity, time);
                index += 6;

                cursor.X += _scale * glyph.Advance;
            }
            return index;
        }

        private static void AddGlyph(
            TextParticleVertex3[] buffer, 
            int index,
            Glyph glyph,
            float outline,
            Color4 color, 
            Vector3 cursor, 
            Vector3 velocity,
            int time)
        {
            float top = cursor.Z + _scale * (glyph.Bounds.Min.Y - outline);
            float bottom = cursor.Z + _scale * (glyph.Bounds.Max.Y + outline);
            float left = cursor.X + _scale * (glyph.Bounds.Min.X - outline);
            float right = cursor.X + _scale * (glyph.Bounds.Max.X + outline);

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
        }
    }
}
