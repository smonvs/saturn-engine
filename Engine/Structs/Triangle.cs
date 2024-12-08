using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaturnEngine.Engine.Structs
{
    public struct Triangle
    {

        public Vector3[] Vertices { get; } = new Vector3[3];
        public Vector2[] TexCoords { get; set; } = new Vector2[3];
        public Color Color { get; set; } = Color.White;

        public Triangle()
        {
            Vertices[0] = new Vector3();
            Vertices[1] = new Vector3();
            Vertices[2] = new Vector3();
            TexCoords[0] = new Vector2();
            TexCoords[1] = new Vector2();
            TexCoords[2] = new Vector2();
        }   

        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vertices[0] = v1; 
            Vertices[1] = v2; 
            Vertices[2] = v3;
            TexCoords[0] = new Vector2(0.0f, 0.0f);
            TexCoords[1] = new Vector2(0.0f, 0.0f);
            TexCoords[2] = new Vector2(0.0f, 0.0f);
        }

        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector2 t1, Vector2 t2, Vector2 t3)
        {
            Vertices[0] = v1;
            Vertices[1] = v2;
            Vertices[2] = v3;
            TexCoords[0] = t1;
            TexCoords[1] = t2;
            TexCoords[2] = t3;
        }

        public static int ClipAgainstPlane(Vector3 planeP, Vector3 planeN, Triangle inTri, out Triangle outTri1, out Triangle outTri2)
        {
            outTri1 = new Triangle();
            outTri2 = new Triangle();

            // Make sure plane ormal is indeed ormal
            planeN.Normalize();

            // Return signed shortest distance from point to plane, plane ormal must be ormalized
            Func<Vector3, float> dist = (Vector3 p) =>
            {
                return (planeN.X * p.X + planeN.Y * p.Y + planeN.Z * p.Z - Vector3.Dot(planeN, planeP));
            };

            // Create two temporary storage arrays to classify points either side of plane
            // If distance sign is positive, point lies on "inside" of plane
            Vector3[] insidePoints = new Vector3[3];
            int insidePointCount = 0;

            Vector3[] outsidePoints = new Vector3[3];
            int outsidePointCount = 0;

            Vector2[] insideTex = new Vector2[3];
            int insideTexCount = 0;

            Vector2[] outsideTex = new Vector2[3];
            int outsideTexCount = 0;

            // Get signed distance of each point in triangle to plane
            float d0 = dist(inTri.Vertices[0]);
            float d1 = dist(inTri.Vertices[1]);
            float d2 = dist(inTri.Vertices[2]);

            if (d0 >= 0) { insidePoints[insidePointCount++] = inTri.Vertices[0]; insideTex[insideTexCount++] = inTri.TexCoords[0]; }
            else { outsidePoints[outsidePointCount++] = inTri.Vertices[0]; outsideTex[outsideTexCount++] = inTri.TexCoords[0]; }
            if (d1 >= 0) { insidePoints[insidePointCount++] = inTri.Vertices[1]; insideTex[insideTexCount++] = inTri.TexCoords[1]; }
            else { outsidePoints[outsidePointCount++] = inTri.Vertices[1]; outsideTex[outsideTexCount++] = inTri.TexCoords[1]; }
            if (d2 >= 0) { insidePoints[insidePointCount++] = inTri.Vertices[2]; insideTex[insideTexCount++] = inTri.TexCoords[2]; }
            else { outsidePoints[outsidePointCount++] = inTri.Vertices[2]; outsideTex[outsideTexCount++] = inTri.TexCoords[2]; }

            // ow classify triangle points, and break the input triangle into 
            // smaller output triangles if required. There are four possible
            // outcomes...

            if (insidePointCount == 0)
            {
                // All points lie on the outside of plane, so clip whole triangle
                // It ceases to exist
                return 0; // o returned triangles are valid
            }

            if (insidePointCount == 3)
            {
                // All points lie on the inside of plane, so do othing
                // and allow the triangle to simply pass through
                outTri1 = inTri;
                return 1; // Just the one returned original triangle is valid
            }

            if (insidePointCount == 1 && outsidePointCount == 2)
            {
                // Triangle should be clipped. As two points lie outside
                // the plane, the triangle simply becomes a smaller triangle

                // Copy appearance info to new triangle
                outTri1.Color = inTri.Color;
                //outTri1.Color = Color.Blue;

                // The inside point is valid, so keep that...
                outTri1.Vertices[0] = insidePoints[0];
                outTri1.TexCoords[0] = insideTex[0];

                // but the two new points are at the locations where the 
                // original sides of the triangle (lines) intersect with the plane
                float t = 0.0f;
                
                outTri1.Vertices[1] = Vector3.IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[0], ref t);
                outTri1.TexCoords[1].X = t * (outsideTex[0].X - insideTex[0].X) + insideTex[0].X;
                outTri1.TexCoords[1].Y = t * (outsideTex[0].Y - insideTex[0].Y) + insideTex[0].Y;

                outTri1.Vertices[2] = Vector3.IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[1], ref t);
                outTri1.TexCoords[2].X = t * (outsideTex[0].X - insideTex[0].X) + insideTex[0].X;
                outTri1.TexCoords[2].Y = t * (outsideTex[0].Y - insideTex[0].Y) + insideTex[0].Y;

                return 1; // Return the newly formed single triangle
            }

            if (insidePointCount == 2 && outsidePointCount == 1)
            {
                // Triangle should be clipped. As two points lie inside the plane,
                // the clipped triangle becomes a "quad". Fortunately, we can
                // represent a quad with two new triangles

                // Copy appearance info to new triangles
                outTri1.Color = inTri.Color;
                outTri2.Color = inTri.Color;
                //outTri1.Color = Color.Green;
                //outTri2.Color = Color.Red;

                // The first triangle consists of the two inside points and a new
                // point determined by the location where one side of the triangle
                // intersects with the plane
                float t = 0.0f;

                outTri1.Vertices[0] = insidePoints[0];
                outTri1.Vertices[1] = insidePoints[1];
                outTri1.TexCoords[0] = insideTex[0];
                outTri1.TexCoords[1] = insideTex[1];
                outTri1.Vertices[2] = Vector3.IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[0], ref t);
                outTri1.TexCoords[2].X = t * (outsideTex[0].X - insideTex[0].X) + insideTex[0].X;
                outTri1.TexCoords[2].Y = t * (outsideTex[0].Y - insideTex[0].Y) + insideTex[0].Y;

                // The second triangle is composed of one of the inside points, a
                // new point determined by the intersection of the other side of the 
                // triangle and the plane, and the newly created point above
                outTri2.Vertices[0] = insidePoints[1];
                outTri2.Vertices[1] = outTri1.Vertices[2];
                outTri2.Vertices[2] = Vector3.IntersectPlane(planeP, planeN, insidePoints[1], outsidePoints[0], ref t);
                outTri2.TexCoords[2].X = t * (outsideTex[0].X - insideTex[0].X) + insideTex[0].X;
                outTri2.TexCoords[2].Y = t * (outsideTex[0].Y - insideTex[0].Y) + insideTex[0].Y;

                return 2; // Return two newly formed triangles which form a quad
            }

            return 0;
        }


    }
}
