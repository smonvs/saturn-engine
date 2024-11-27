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
        public Color Color { get; set; } = Color.White;

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

        public static int ClipAgainstPlane(Vector3 planeP, Vector3 planeN, ref Triangle inTri, ref Triangle outTri1, ref Triangle outTri2)
        {
            // Make sure plane normal is indeed normal
            planeN.Normalize();

            // Return signed shortest distance from point to plane, plane normal must be normalized
            Func<Vector3, float> dist = (Vector3 p) =>
            {
                return (planeN.X * p.X + planeN.Y * p.Y + planeN.Z * p.Z - Vector3.Dot(planeN, planeP));
            };

            // Create two temporary storage arrays to classify points either side of plane
            // If distance sign is positive, point lies on "inside" of plane
            Vector3[] insidePoints = new Vector3[3];
            int nInsidePointCount = 0;
            Vector3[] outsidePoints = new Vector3[3];
            int nOutsidePointCount = 0;

            // Get signed distance of each point in triangle to plane
            float d0 = dist(inTri.Vertices[0]);
            float d1 = dist(inTri.Vertices[1]);
            float d2 = dist(inTri.Vertices[2]);

            if (d0 >= 0) { insidePoints[nInsidePointCount++] = inTri.Vertices[0]; }
            else { outsidePoints[nOutsidePointCount++] = inTri.Vertices[0]; }
            if (d1 >= 0) { insidePoints[nInsidePointCount++] = inTri.Vertices[1]; }
            else { outsidePoints[nOutsidePointCount++] = inTri.Vertices[1]; }
            if (d2 >= 0) { insidePoints[nInsidePointCount++] = inTri.Vertices[2]; }
            else { outsidePoints[nOutsidePointCount++] = inTri.Vertices[2]; }

            // Now classify triangle points, and break the input triangle into 
            // smaller output triangles if required. There are four possible
            // outcomes...

            if (nInsidePointCount == 0)
            {
                // All points lie on the outside of plane, so clip whole triangle
                // It ceases to exist
                return 0; // No returned triangles are valid
            }

            if (nInsidePointCount == 3)
            {
                // All points lie on the inside of plane, so do nothing
                // and allow the triangle to simply pass through
                outTri1 = inTri;
                return 1; // Just the one returned original triangle is valid
            }

            if (nInsidePointCount == 1 && nOutsidePointCount == 2)
            {
                // Triangle should be clipped. As two points lie outside
                // the plane, the triangle simply becomes a smaller triangle

                // Copy appearance info to new triangle
                outTri1.Color = inTri.Color;

                // The inside point is valid, so keep that...
                outTri1.Vertices[0] = insidePoints[0];

                // but the two new points are at the locations where the 
                // original sides of the triangle (lines) intersect with the plane
                outTri1.Vertices[1] = Vector3.IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[0]);
                outTri1.Vertices[2] = Vector3.IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[1]);

                return 1; // Return the newly formed single triangle
            }

            if (nInsidePointCount == 2 && nOutsidePointCount == 1)
            {
                // Triangle should be clipped. As two points lie inside the plane,
                // the clipped triangle becomes a "quad". Fortunately, we can
                // represent a quad with two new triangles

                // Copy appearance info to new triangles
                outTri1.Color = inTri.Color;
                outTri2.Color = inTri.Color;

                // The first triangle consists of the two inside points and a new
                // point determined by the location where one side of the triangle
                // intersects with the plane
                outTri1.Vertices[0] = insidePoints[0];
                outTri1.Vertices[1] = insidePoints[1];
                outTri1.Vertices[2] = Vector3.IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[0]);

                // The second triangle is composed of one of the inside points, a
                // new point determined by the intersection of the other side of the 
                // triangle and the plane, and the newly created point above
                outTri2.Vertices[0] = insidePoints[1];
                outTri2.Vertices[1] = outTri1.Vertices[2];
                outTri2.Vertices[2] = Vector3.IntersectPlane(planeP, planeN, insidePoints[1], outsidePoints[0]);

                return 2; // Return two newly formed triangles which form a quad
            }

            return 0;
        }


    }
}
