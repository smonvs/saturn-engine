namespace SaturnEngine.Engine.Structs
{
    public struct Quaternion
    {

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Quaternion()
        {
            X = 0.0f;
            Y = 0.0f;
            Z = 0.0f;
            W = 1.0f;
        }

        public Quaternion(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            W = 1.0f;
        }

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        #region Operators

        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y, q1.W * q2.Y + q1.Y * q2.W + q1.Z * q2.X - q1.X * q2.Z, q1.W * q2.Z + q1.Z * q2.W + q1.X * q2.Y - q1.Y * q2.X, q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z);
        }

        #endregion

        public void Normalize()
        {
            float length = MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);
            if(length > 0.0)
            {
                X /= length;
                Y /= length;
                Z /= length;
                W /= length;
            }
        }

        public Vector3 ToEuler()
        {
            Vector3 angles = new Vector3();

            float sinz_cosp = 2 * (W * X + Y * Z);
            float cosz_cosp = 1 - 2 * (X * X + Y * Y);
            angles.Z = MathF.Atan2(sinz_cosp, cosz_cosp);

            float sinx = 2 * (W * Y - Z * X);
            if(Math.Abs(sinx) >= 1)
            {
                angles.X = MathF.PI / 2 * Math.Sign(sinx);
            }
            else
            {
                angles.X = MathF.Asin(sinx);
            }

            float siny_cosp = 2 * (W * Z + X * Y);
            float cosy_cosp = 1 - 2 * (Y * Y + Z * Z);
            angles.Y = MathF.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }

    }
}
