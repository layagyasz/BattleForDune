using Cardamom;
using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BattleForDune.View
{
    public class MapLayer : ManagedResource, IRenderable
    {
        public record class Factory(RenderShader GridShader)
        {
            private static readonly float _sqrt3_2 = 0.5f * MathF.Sqrt(3);
            private static readonly Vector3[] _corners =
            [
                new(-0.5f, 0, -_sqrt3_2),
                new(0.5f, 0, -_sqrt3_2),
                new(1, 0, 0),
                new(0.5f, 0, _sqrt3_2),
                new(-0.5f, 0, _sqrt3_2),
                new(-1, 0, 0)
            ];
            private static readonly Color4 _gridColor = Color4.Gray;
            private const float _gridWidth = 0.04f;

            public MapLayer Create(Vector2i size)
            {
                ArrayList<Vertex3> grid = new(36);
                Shapes.AddVertices(
                    grid,
                    _gridColor,
                    new Line3(_corners, Vector3.UnitY, isLoop: true),
                    _gridWidth,
                    center: false);

                var instances = new InstanceVertex3[size.X * size.Y];
                for (int i = 0; i < size.X; ++i)
                {
                    for (int j = 0; j < size.Y; ++j)
                    {
                        instances[j * size.X + i] = 
                            new(Geometry.Projection.AxialToCartesian(
                                Geometry.Projection.OffsetToAxial(new(i, j))));
                    }
                }

                var vao = new InstanceVertexBuffer<Vertex3, InstanceVertex3>(PrimitiveType.Triangles);
                vao.BufferMesh(grid.GetData());
                vao.Buffer(instances);
                return new(vao, GridShader);
            }
        }

        private readonly InstanceVertexBuffer<Vertex3, InstanceVertex3> _grid;
        private readonly RenderShader _gridShader;

        private MapLayer(InstanceVertexBuffer<Vertex3, InstanceVertex3> grid, RenderShader gridShader)
        {
            _grid = grid;
            _gridShader = gridShader;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.Draw(_grid, 0, _grid.Length, new(BlendMode.None, _gridShader));
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 context) { }

        public void Update(long delta) { }

        protected override void DisposeImpl()
        {
            _grid.Dispose();
        }
    }
}
