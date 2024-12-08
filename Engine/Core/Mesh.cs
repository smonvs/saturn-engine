using SaturnEngine.Engine.Structs;

namespace SaturnEngine.Engine.Core
{
    public class Mesh : Resource
    {

        public Triangle[] Triangles { get; }

        #region Static Meshes

        public static readonly Mesh Cube = new Mesh(new Triangle[]
        {
            // South
            new Triangle(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0)),
            new Triangle(new Vector3(0, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)),
            // East
            new Triangle(new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0)),
            new Triangle(new Vector3(1, 0, 0), new Vector3(1, 1, 1), new Vector3(1, 0, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)),
            // North
            new Triangle(new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0)),
            new Triangle(new Vector3(1, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 0, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)),
            // West
            new Triangle(new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 0), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0)),
            new Triangle(new Vector3(0, 0, 1), new Vector3(0, 1, 0), new Vector3(0, 0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)),
            // Top
            new Triangle(new Vector3(0, 1, 0), new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0)),
            new Triangle(new Vector3(0, 1, 0), new Vector3(1, 1, 1), new Vector3(1, 1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)),
            // Bottom
            new Triangle(new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0)),
            new Triangle(new Vector3(1, 0, 1), new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1))
        });
        public static readonly Mesh Plane = new Mesh(new Triangle[]
        {
            new Triangle(new Vector3(0, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 0, 0)),
            new Triangle(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 1))
        });

        #endregion

        public Mesh(Triangle[] tris, string? filepath = null) : base(filepath)
        {
            Triangles = tris;
        }

    }
}
