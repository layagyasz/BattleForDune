using Cardamom.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace BattleForDune.View
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TextParticleVertex3(
        Vector3 position, Color4 color, Vector2 texCoords, Vector3 velocity, int birthTime)
    {
        [VertexAttribute(0, 3, VertexAttribPointerType.Float, false)]
        public Vector3 Position = position;

        [VertexAttribute(1, 4, VertexAttribPointerType.Float, false)]
        public Color4 Color = color;

        [VertexAttribute(2, 2, VertexAttribPointerType.Float, false)]
        public Vector2 TexCoords = texCoords;

        [VertexAttribute(3, 3, VertexAttribPointerType.Float, false)]
        public Vector3 Velocity = velocity;

        [VertexAttribute(4, 1, VertexAttribPointerType.Int, false)]
        public int BirthTime = birthTime;
    }
}
