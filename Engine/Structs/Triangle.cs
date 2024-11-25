using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace SaturnEngine.Engine.Structs
{
    public struct Triangle
    {

        public Vector3[] Vertices { get; } = new Vector3[3];
        public Color color;

        public Triangle()
        {
            Vertices[0] = new Vector3();
            Vertices[1] = new Vector3();
            Vertices[2] = new Vector3();
        }   

        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vertices[0] = v1; 
            Vertices[1] = v2; 
            Vertices[2] = v3;
        }
    }
}
