using BattleForDune.Core;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;
using BattleForDune.Geometry;
using Cardamom;
using Cardamom.Graphics;
using Cardamom.Mathematics.Color;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace BattleForDune.View.Layers
{
    public class HealthLayer(RenderShader shader, int capacity)
        : ManagedResource, IRenderable
    {
        public record class Factory(RenderShader Shader)
        {
            public HealthLayer Create(int capacity)
            {
                return new(Shader, capacity);
            }
        }

        private static readonly float sqrt3_2 = 0.5f * MathF.Sqrt(3);
        private static readonly Vector3[] _corners =
        [
            new(0f, 0f, 0f),
            new(1f, 0f, 0f),
            new(0f, 0f, 1f),
            new(0f, 0f, 1f),
            new(1f, 0f, 0f),
            new(1f, 0f, 1f)
        ];
        private static readonly Vector3 _offset = new(0, 0, -1.75f * sqrt3_2);
        private static readonly Vector3 _size = new(1.25f, 0, 0.05f);
        private static readonly Vector4 _maxColor = ColorToVector(new Color4(0f, 1f, 0f, 1f).AsHsl());
        private static readonly Vector4 _minColor = ColorToVector(new Color4(1f, 0f, 0f, 1f).AsHsl());

        private static readonly Vertex3[] _buffer = new Vertex3[6];

        private readonly DenseVertexBuffer<Vertex3> _vertices = new(capacity, 6);
        private readonly UniqueIndex _index = new(capacity);
        private readonly RenderShader _shader = shader;

        public void Add(EntityId unitId, Stat stat, Hex2b position)
        {
            SetVertices(
                _buffer,
                0,
                Geometry.Projection.AxialToCartesian(position),
                stat);
            _vertices.Allocate(out var handle);
            _vertices.Set(handle, _buffer);

            _index.Add(unitId.Value, handle);
        }

        public void Move(EntityId unitId, Stat stat, Hex2b position)
        {
            if (_index.TryGet(unitId.Value, out var handle))
            {
                SetVertices(
                    _buffer,
                    0,
                    Geometry.Projection.AxialToCartesian(position),
                    stat);
                _vertices.Set(handle, _buffer);
            }
        }

        public void Remove(EntityId unitId)
        {
            if (_index.RemoveOnly(unitId.Value, out var handle))
            {
                _vertices.Free(handle);
            }
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.Draw(
                _vertices.GetBuffer(),
                0,
                6 * _vertices.Count,
                new(BlendMode.Alpha, _shader));
        }

        public void Initialize()
        {
            _vertices.Initialize();
        }

        public void ResizeContext(Vector3 context) { }

        public void Update(long delta) { }

        private static void SetVertices(Vertex3[] vertices, int offset, Vector3 position, Stat stat)
        {
            var topLeft = position + _offset + new Vector3(-0.5f * _size.X, 0, -_size.Y);
            var pct = Math.Clamp(1f * stat.Total / stat.Max, 0, 1);
            SetVertices(
                vertices,
                offset,
                topLeft, 
                _size * new Vector3(pct, 1, 1), 
                VectorToColor(_minColor + pct * (_maxColor - _minColor)).AsRgb());
        }

        private static void SetVertices(
            Vertex3[] vertices, int offset, Vector3 position, Vector3 size, Color4 color)
        {
            vertices[offset] = new(position + _corners[0], color, new());
            vertices[offset + 1] =
                new(position + size * _corners[1], color, new());
            vertices[offset + 2] =
                new(position + size * _corners[2], color, new());
            vertices[offset + 3] =
                new(position + size * _corners[3], color, new());
            vertices[offset + 4] =
                new(position + size *  _corners[4], color, new());
            vertices[offset + 5] = new(position + size * _corners[5], color, new());
        }

        private static Vector4 ColorToVector(ColorHsl color)
        {
            return new(color.H, color.S, color.L, color.A);
        }

        private static ColorHsl VectorToColor(Vector4 vector)
        {
            return new(vector.X, vector.Y, vector.Z, vector.W);
        }

        protected override void DisposeImpl()
        {
            _vertices.Dispose();
        }
    }
}
