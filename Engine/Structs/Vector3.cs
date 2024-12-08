using System;
using System.ComponentModel.DataAnnotations;

namespace SaturnEngine.Engine.Structs
{
    public struct Vector3
    {

        #region Statics

        public static readonly Vector3 Zero = new Vector3(0, 0, 0);
        public static readonly Vector3 Up = new Vector3(0, -1, 0);
        public static readonly Vector3 Down = new Vector3(0, 1, 0);

        #endregion

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; internal set; }

        public float Length { get { return MathF.Sqrt(Dot(this, this)); } }

        public Vector3()
        {
            X = 0;
            Y = 0;
            Z = 0;
            W = 1;
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            W = 1;
        }

        internal Vector3(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        #region Operators

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3 operator +(Vector3 v, float f)
        {
            return new Vector3(v.X + f, v.Y + f, v.Z + f);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3 operator -(Vector3 v, float f)
        {
            return new Vector3(v.X - f, v.Y - f, v.Z - f);
        }

        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.X, -v.Y, -v.Z);
        }

        public static Vector3 operator *(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        public static Vector3 operator *(Vector3 v, float f)
        {
            return new Vector3(v.X * f, v.Y * f, v.Z * f);
        }

        public static Vector3 operator /(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
        }

        public static Vector3 operator /(Vector3 v, float f)
        {
            return new Vector3(v.X / f, v.Y / f, v.Z / f);
        }

        #endregion

        public void Normalize()
        {
            float length = Length;
            X /= length;
            Y /= length; 
            Z /= length;
        }

        public static float Dot(Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            return new Vector3
            (
                v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X
            );
        }

        public static Vector3 IntersectPlane(ref readonly Vector3 planeP, Vector3 planeN, ref readonly Vector3 lineStart, ref readonly Vector3 lineEnd, ref float t)
        {
            planeN.Normalize();
            
            float planeD = -Dot(planeN, planeP);
            float ad = Dot(lineStart, planeN);
            float bd = Dot(lineEnd, planeN);
            t = (-planeD - ad) / (bd - ad);

            Vector3 lineStartToEnd = lineEnd - lineStart;
            Vector3 lineToIntersect = lineStartToEnd * t;

            return lineStart + lineToIntersect;
        }

    }
}
