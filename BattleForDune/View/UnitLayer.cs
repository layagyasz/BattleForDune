using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace BattleForDune.View
{
    public class UnitLayer(UnitTextureCache textures, RenderShader shader, int capacity) : IRenderable
    {
        private readonly UnitTextureCache _textures = textures;
        private readonly DenseVertexBuffer<Vertex3> _vertices = new(capacity, 6);
        private readonly RenderShader _shader = shader;

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.Draw(
                _vertices.GetBuffer(), 
                0, 
                _vertices.Count, new(BlendMode.Alpha, _shader, _textures.GetTexture()));
        }

        public void Initialize()
        {
            _vertices.Initialize();
        }

        public void ResizeContext(Vector3 context) { }

        public void Update(long delta) { }
    }
}
