using BattleForDune.Core;
using BattleForDune.FastLookup;
using BattleForDune.Geometry;
using Cardamom;
using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace BattleForDune.View
{
    public class AuraLayer(TileTextureCache textures, RenderShader shader, int capacity)
            : ManagedResource, IRenderable
    {
        public record class Factory(RenderShader Shader)
        {
            public AuraLayer Create(TileTextureCache textures, int capacity)
            {
                return new(textures, Shader, capacity);
            }
        }

        private static readonly Vertex3[] _buffer = new Vertex3[18];

        private readonly ref struct AddHexCallback(
            EntityId auraId, Box2i texBox, DenseVertexBuffer<Vertex3> vertices, MultiIndex index) 
            : ICallback<Hex2b>
        {
            private readonly EntityId _auraId = auraId;
            private readonly Box2i _texBox = texBox;
            private readonly DenseVertexBuffer<Vertex3> _vertices = vertices;
            private readonly MultiIndex _index = index;

            public readonly void Notify(in Hex2b hex)
            {
                SetVertices(
                    _buffer,
                    0,
                    Geometry.Projection.AxialToCartesian(hex),
                    Color4.White,
                    _texBox);
                _vertices.Allocate(out var handle);
                _vertices.Set(handle, _buffer);
                _index.Add(_auraId.Value, handle);
            }
        }

        private readonly TileTextureCache _textures = textures;
        private readonly DenseVertexBuffer<Vertex3> _vertices = new(capacity, 18);
        private readonly MultiIndex _index = new(capacity, capacity);
        private readonly RenderShader _shader = shader;

        public void Add(EntityId auraId, RegistryKey prototypeKey, Hex2b position, byte range)
        {
            var callback = new AddHexCallback(auraId, _textures.Get(prototypeKey), _vertices, _index);
            Hexes.Range(position, range, ref callback);
        }

        public void Remove(EntityId auraId)
        {
            foreach (var handle in _index.GetEnumerator(auraId.Value))
            {
                _vertices.Free(handle);
            }
            _index.RemoveAll(auraId.Value);
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.Draw(
                _vertices.GetBuffer(),
                0,
                18 * _vertices.Count,
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
            // TODO: Support rotations/reflections
            for (int t = 0; t < 6; ++t)
            {
                vertices[offset + 3 * t] = new(position, Color4.White, textureView.Center);
                vertices[offset + 3 * t + 1] =
                    new(
                        position + Hexes.Corners[t],
                        Color4.White,
                        textureView.Center + textureView.HalfSize * Hexes.Corners[t].Xz);
                vertices[offset + 3 * t + 2] =
                    new(
                        position + Hexes.Corners[(t + 1) % 6],
                        Color4.White,
                        textureView.Center + textureView.HalfSize * Hexes.Corners[(t + 1) % 6].Xz);
            }
        }

        protected override void DisposeImpl()
        {
            _vertices.Dispose();
        }
    }
}
