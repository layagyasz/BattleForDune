using Cardamom.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace BattleForDune.View
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InstanceVertex3(Vector3 position)
    {
        [VertexAttribute(3, 3, VertexAttribPointerType.Float, false)]
        public Vector3 Position = position;
    }
}
