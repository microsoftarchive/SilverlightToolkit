using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Silverlight3dApp
{
    /// <summary>
    /// Define a vertex with normal (for lighting) and local color
    /// </summary>
    public struct VertexPositionColorNormal
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 Color;

        public const int Stride = 40;

        /// <summary>
        /// Constructor.
        /// </summary>
        public VertexPositionColorNormal(Vector3 position, Vector3 normal, Vector4 color)
        {
            Position = position;
            Normal = normal;
            Color = color;
        }

        /// <summary>
        /// A VertexDeclaration object, which contains information about the vertex
        /// elements contained within this struct.
        /// </summary>
        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector4, VertexElementUsage.Color, 0)
        );
    }
}
