using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaturnEngine.Engine.Structs
{
    public struct Matrix4x4
    {

        public float[,] M { get; private set; }

        #region Static
        
            public static readonly Matrix4x4 Identity = new Matrix4x4()
            {
                M = new float[4, 4]
                {
                    {1, 0, 0, 0},
                    {0, 1, 0, 0},
                    {0, 0, 1, 0},
                    {0, 0, 0, 1}
                }
            };

        #endregion

        public Matrix4x4()
        {
            M = new float[4, 4];
        }

        public static Vector3 MultiplyVector(Matrix4x4 m, Vector3 v)
        {
            Vector3 vector = new Vector3();
            vector.X = v.X * m.M[0, 0] + v.Y * m.M[1, 0] + v.Z * m.M[2, 0] + v.W * m.M[3, 0];
            vector.Y = v.X * m.M[0, 1] + v.Y * m.M[1, 1] + v.Z * m.M[2, 1] + v.W * m.M[3, 1];
            vector.Z = v.X * m.M[0, 2] + v.Y * m.M[1, 2] + v.Z * m.M[2, 2] + v.W * m.M[3, 2];
            vector.W = v.X * m.M[0, 3] + v.Y * m.M[1, 3] + v.Z * m.M[2, 3] + v.W * m.M[3, 3];
            return vector;
        }
        
        public static Matrix4x4 MultiplyMatrix(Matrix4x4 m1, Matrix4x4 m2)
        {
            Matrix4x4 matrix = new Matrix4x4();
            for(int c = 0; c < 4; c++)
            {
                for(int r = 0; r < 4; r++)
                {
                    matrix.M[r, c] = m1.M[r, 0] * m2.M[0, c] + m1.M[r, 1] * m2.M[1, c] + m1.M[r, 2] * m2.M[2, c] + m1.M[r, 3] * m2.M[3, c];
                }
            }
            return matrix;
        }

        public static Matrix4x4 MakeRotationX(float fAngleRad)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.M[0, 0] = 1.0f;
            matrix.M[1, 1] = MathF.Cos(fAngleRad);
            matrix.M[1, 2] = MathF.Sin(fAngleRad);
            matrix.M[2, 1] = -MathF.Sin(fAngleRad);
            matrix.M[2, 2] = MathF.Cos(fAngleRad);
            matrix.M[3, 3] = 1.0f;
            return matrix;
        }

        public static Matrix4x4 MakeRotationY(float fAngleRad)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.M[0, 0] = MathF.Cos(fAngleRad);
            matrix.M[0, 2] = MathF.Sin(fAngleRad);
            matrix.M[2, 0] = -MathF.Sin(fAngleRad);
            matrix.M[1, 1] = 1.0f;
            matrix.M[2, 2] = MathF.Cos(fAngleRad);
            matrix.M[3, 3] = 1.0f;
            return matrix;
        }

        public static Matrix4x4 MakeRotationZ(float fAngleRad)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.M[0, 0] = MathF.Cos(fAngleRad);
            matrix.M[0, 1] = MathF.Sin(fAngleRad);
            matrix.M[1, 0] = -MathF.Sin(fAngleRad);
            matrix.M[1, 1] = MathF.Cos(fAngleRad);
            matrix.M[2, 2] = 1.0f;
            matrix.M[3, 3] = 1.0f;
            return matrix;
        }

        public static Matrix4x4 MakeTranslation(float x, float y, float z)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.M[0, 0] = 1.0f;
            matrix.M[1, 1] = 1.0f;
            matrix.M[2, 2] = 1.0f;
            matrix.M[3, 3] = 1.0f;
            matrix.M[3, 0] = x;
            matrix.M[3, 1] = y;
            matrix.M[3, 2] = z;
            return matrix;
        }

        public static Matrix4x4 MakeProjection(float fov, float fAspectRatio, float fNear, float fFar)
        {
            float fFovRad = 1.0f / MathF.Tan(fov * 0.5f / 180.0f * MathF.PI);
            Matrix4x4 matrix = new Matrix4x4();
            matrix.M[0, 0] = fAspectRatio * fFovRad;
            matrix.M[1, 1] = fFovRad;
            matrix.M[2, 2] = fFar / (fFar - fNear);
            matrix.M[3, 2] = (-fFar * fNear) / (fFar - fNear);
            matrix.M[2, 3] = 1.0f;
            matrix.M[3, 3] = 0.0f;
            return matrix;
        }

        public static Matrix4x4 PointAt(Vector3 position, Vector3 target, Vector3 up)
        {
            Vector3 newForward = target - position;
            newForward.Normalize();

            Vector3 a = newForward * Vector3.Dot(up, newForward);
            Vector3 newUp = up - a;

            Vector3 newRight = Vector3.Cross(newUp, newForward);

            Matrix4x4 matrix = new Matrix4x4();
            matrix.M[0, 0] = newRight.X; matrix.M[0, 1] = newRight.Y; matrix.M[0, 2] = newRight.Z; matrix.M[0, 3] = 0.0f;
            matrix.M[1, 0] = newUp.X; matrix.M[1, 1] = newUp.Y; matrix.M[1, 2] = newUp.Z; matrix.M[1, 3] = 0.0f;
            matrix.M[2, 0] = newForward.X; matrix.M[2, 1] = newForward.Y; matrix.M[2, 2] = newForward.Z; matrix.M[2, 3] = 0.0f;
            matrix.M[3, 0] = position.X; matrix.M[3, 1] = position.Y; matrix.M[3, 2] = position.Z; matrix.M[3, 3] = 1.0f;
            return matrix;
        }

        public static Matrix4x4 QuickInverse(Matrix4x4 matrix)
        {
            Matrix4x4 matrixInverse = new Matrix4x4();
            matrixInverse.M[0, 0] = matrix.M[0, 0]; matrixInverse.M[0, 1] = matrix.M[1, 0]; matrixInverse.M[0, 2] = matrix.M[2, 0]; matrixInverse.M[0, 3] = 0.0f;
            matrixInverse.M[1, 0] = matrix.M[0, 1]; matrixInverse.M[1, 1] = matrix.M[1, 1]; matrixInverse.M[1, 2] = matrix.M[2, 1]; matrixInverse.M[1, 3] = 0.0f;
            matrixInverse.M[2, 0] = matrix.M[0, 2]; matrixInverse.M[2, 1] = matrix.M[1, 2]; matrixInverse.M[2, 2] = matrix.M[2, 2]; matrixInverse.M[2, 3] = 0.0f;
            matrixInverse.M[3, 0] = -(matrix.M[3, 0] * matrix.M[0, 0] + matrix.M[3, 1] * matrix.M[1, 0] + matrix.M[3, 2] * matrix.M[2, 0]);
            matrixInverse.M[3, 1] = -(matrix.M[3, 0] * matrix.M[0, 1] + matrix.M[3, 1] * matrix.M[1, 1] + matrix.M[3, 2] * matrix.M[2, 1]);
            matrixInverse.M[3, 2] = -(matrix.M[3, 0] * matrix.M[0, 2] + matrix.M[3, 1] * matrix.M[1, 2] + matrix.M[3, 2] * matrix.M[2, 2]);
            matrixInverse.M[3, 3] = 1.0f;
            return matrixInverse;   
        }

    }
}
