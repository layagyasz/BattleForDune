using BattleForDune.Core;
using BattleForDune.FastLookup;
using BattleForDune.Geometry;
using Cardamom;
using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace BattleForDune.View
{
    public class UnitLayer(UnitTextureCache textures, RenderShader shader, int capacity) 
        : ManagedResource, IRenderable
    {
        public record class Factory(RenderShader Shader)
        {
            public UnitLayer Create(UnitTextureCache textures, int capacity)
            {
                return new(textures, Shader, capacity);
            }
        }

        private static readonly float sqrt3_2 = 0.5f * MathF.Sqrt(3);
        private static readonly Vector3[] _corners =
        [
            new(-1, 0f, -1f),
            new(1, 0f, -1f),
            new(-1, 0f, 1f),
            new(-1, 0f, 1f),
            new(1, 0f, -1f),
            new(1, 0f, 1f)
        ];
        private static readonly Vector3 _offset = new(0, 0, -0.5f * sqrt3_2);
        private static readonly Vertex3[] _buffer = new Vertex3[6];

        private readonly UnitTextureCache _textures = textures;
        private readonly DenseVertexBuffer<Vertex3> _vertices = new(capacity, 6);
        private readonly UniqueIndex _index = new(capacity);
        // TODO: Make indices generic
        private readonly UniqueIndex _prototypes = new(capacity);
        private readonly RenderShader _shader = shader;

        public void Add(EntityId unitId, RegistryKey prototypeKey, Hex2b position)
        {
            SetVertices(
                _buffer, 
                0, 
                Geometry.Projection.AxialToCartesian(position) + _offset,
                Color4.White, 
                _textures.Get(prototypeKey));
            _vertices.Allocate(out var handle);
            _vertices.Set(handle, _buffer);

            _index.Add(unitId.Value, handle);
            _prototypes.Add(unitId.Value, prototypeKey.Value);
        }

        public void Move(EntityId unitId, Hex2b position)
        {
            if (_index.TryGet(unitId.Value, out var handle) 
                && _prototypes.TryGet(unitId.Value, out var prototypeKey))
            {
                SetVertices(
                    _buffer,
                    0,
                    Geometry.Projection.AxialToCartesian(position) + _offset,
                    Color4.White,
                    _textures.Get(new RegistryKey((ushort)prototypeKey)));
                _vertices.Set(handle, _buffer);
            }
        }

        public void Remove(EntityId unitId)
        {
            if (_index.RemoveOnly(unitId.Value, out var handle))
            {
                _vertices.Free(handle);
                _prototypes.RemoveOnly(unitId.Value, out var _);
            }
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.Draw(
                _vertices.GetBuffer(), 
                0, 
                6 * _vertices.Count,
                new(BlendMode.Alpha, _shader, _textures.GetTexture()));
        }

        public void Initialize()
        {
            _vertices.Initialize();
        }

        public void ResizeContext(Vector3 context) { }

        public void Update(long delta) { }

        private static void SetVertices(
            Vertex3[] vertices, int offset, Vector3 position, Color4 color, Box2i textureView)
        {
            vertices[offset] = new(position + _corners[0], color, textureView.Min);
            vertices[offset + 1] = 
                new(position + _corners[1], color, new(textureView.Max.X, textureView.Min.Y));
            vertices[offset + 2] = 
                new(position + _corners[2], color, new(textureView.Min.X, textureView.Max.Y));
            vertices[offset + 3] = 
                new(position + _corners[3], color, new(textureView.Min.X, textureView.Max.Y));
            vertices[offset + 4] = 
                new(position + _corners[4], color, new(textureView.Max.X, textureView.Min.Y));
            vertices[offset + 5] = new(position + _corners[5], color, textureView.Max);
        }

        protected override void DisposeImpl()
        {
            _vertices.Dispose();
        }
    }
}
