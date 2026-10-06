using BattleForDune.Core.Schema;
using BattleForDune.Geometry;
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
        public record class Factory(RenderShader Shader, RenderShader GridShader)
        {
            private static readonly Color4 _gridColor = Color4.Gray;
            private const float _gridWidth = 0.02f;

            public MapLayer Create(TileTextureCache textures, DenseHexGrid<Tile> map)
            {

                return new(
                    CreateTiles(map, textures), 
                    textures.GetTexture(),
                    Shader, 
                    CreateGrid(map.Size), GridShader);
            }

            private static VertexBuffer<Vertex3> CreateTiles(
                DenseHexGrid<Tile> map, TileTextureCache textures)
            {
                var mesh = new Vertex3[18 * map.Height * map.Width];
                for (int i=0; i<map.Height; ++i)
                {
                    for (int j=0; j<map.Width; ++j)
                    {
                        int tileIndex = 18 * (i * map.Width + j);
                        var center = 
                            Geometry.Projection.AxialToCartesian(Geometry.Projection.OffsetToAxial(new(j, i)));
                        var tile = map.Get(i, j);
                        var segment = textures.Get(tile.TileKey);
                        for (int t=0; t<6; ++t)
                        {
                            mesh[tileIndex + 3 * t] = new(center, Color4.White, segment.Center);
                            mesh[tileIndex + 3 * t + 1] =
                                new(
                                    center + Hexes.Corners[t], 
                                    Color4.White,
                                    segment.Center + segment.HalfSize * Transform(t, tile.Transform));
                            mesh[tileIndex + 3 * t + 2] = 
                                new(
                                    center + Hexes.Corners[(t + 1) % 6], 
                                    Color4.White, 
                                    segment.Center + segment.HalfSize * Transform(t + 1, tile.Transform));
                        }
                    }
                }
                var vao = new VertexBuffer<Vertex3>(PrimitiveType.Triangles);
                vao.Buffer(mesh);
                return vao;
            }

            private static readonly int[] _reflection = [ 1, 0, 5, 4, 3, 2 ];
            private static Vector2 Transform(int index, Tile.Transformation transform)
            {
                index = (index + transform.GetRotation()) % 6;
                if (transform.IsReflected())
                {
                    index = _reflection[index];
                }
                return Hexes.Corners[index].Xz;
            }

            private static InstanceVertexBuffer<Vertex3, InstanceVertex3> CreateGrid(Vector2i size)
            {
                ArrayList<Vertex3> grid = new(36);
                Shapes.AddVertices(
                    grid,
                    _gridColor,
                    new Line3(Hexes.Corners, Vector3.UnitY, isLoop: true),
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
                return vao;
            }
        }

        private readonly VertexBuffer<Vertex3> _tiles;
        private readonly Texture _tileTexture;
        private readonly RenderShader _shader;

        private readonly InstanceVertexBuffer<Vertex3, InstanceVertex3> _grid;
        private readonly RenderShader _gridShader;

        private MapLayer(
            VertexBuffer<Vertex3> tiles, 
            Texture tileTexture,
            RenderShader shader, 
            InstanceVertexBuffer<Vertex3, InstanceVertex3> grid, 
            RenderShader gridShader)
        {
            _tiles = tiles;
            _tileTexture = tileTexture;
            _shader = shader;
            _grid = grid;
            _gridShader = gridShader;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.Draw(_tiles, 0, _tiles.Length, new(BlendMode.Alpha, _shader, _tileTexture));
            target.Draw(_grid, 0, _grid.Length, new(BlendMode.Alpha, _gridShader));
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 context) { }

        public void Update(long delta) { }

        protected override void DisposeImpl()
        {
            _tiles.Dispose();
            _tileTexture.Dispose();
            _grid.Dispose();
        }
    }
}
