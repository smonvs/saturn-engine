using SaturnEngine.Engine.Core;

namespace SaturnEngine.Engine.Structs
{
    public class Mesh : Resource
    {

        public Triangle[] Triangles { get; }

        #region Static Meshes

        public static readonly Mesh Cube = new Mesh(new Triangle[]
        {
            // South
            new Triangle(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0)),
            new Triangle(new Vector3(0, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0)),
            // East
            new Triangle(new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1)),
            new Triangle(new Vector3(1, 0, 0), new Vector3(1, 1, 1), new Vector3(1, 0, 1)),
            // North
            new Triangle(new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1)),
            new Triangle(new Vector3(1, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 0, 1)),
            // West
            new Triangle(new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 0)),
            new Triangle(new Vector3(0, 0, 1), new Vector3(0, 1, 0), new Vector3(0, 0, 0)),
            // Top
            new Triangle(new Vector3(0, 1, 0), new Vector3(0, 1, 1), new Vector3(1, 1, 1)),
            new Triangle(new Vector3(0, 1, 0), new Vector3(1, 1, 1), new Vector3(1, 1, 0)),
            // Bottom
            new Triangle(new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
            new Triangle(new Vector3(1, 0, 1), new Vector3(0, 0, 0), new Vector3(1, 0, 0))
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
