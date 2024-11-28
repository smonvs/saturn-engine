namespace SaturnEngine.Engine.Structs
{
    public struct Quaternion
    {

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float W { get; private set; }

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

            // roll (x-axis rotation)
            double sinr_cosp = 2 * (W * X + Y * Z);
            double cosr_cosp = 1 - 2 * (X * X + Y * Y);
            angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            double sinp = 2 * (W * Y - Z * X);
            if (Math.Abs(sinp) >= 1)
            {
                angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
            }
            else
            {
                angles.Y = (float)Math.Asin(sinp);
            }

            // yaw (z-axis rotation)
            double siny_cosp = 2 * (W * Z + X * Y);
            double cosy_cosp = 1 - 2 * (Y * Y + Z * Z);
            angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }

    }
}
