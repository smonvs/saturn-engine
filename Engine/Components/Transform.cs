using SaturnEngine.Engine.Core;
using SaturnEngine.Engine.Structs;
using System;
using System.Transactions;

namespace SaturnEngine.Engine.Components
{
    public class Transform : ComponentBase
    {

        [Export] public Vector3 Position { get; private set; }
        [Export] public Quaternion Rotation { get; private set; }
        //[Export] public Vector3 Scale { get; private set; }

        public Vector3 Forward
        {
            get
            {
                return new Vector3(
                    2 * (Rotation.X * Rotation.Z - Rotation.W * Rotation.Y),
                    2 * (Rotation.Y * Rotation.Z - Rotation.W * Rotation.X), // Vorzeichen geändert
                    1 - 2 * (Rotation.X * Rotation.X + Rotation.Y * Rotation.Y)
                );
            }
        }

        public override void OnInit()
        {
            Position = Vector3.Zero;
            Rotation = new Quaternion(0, 0, 0, 1);
            //Scale = new Vector3(1, 1, 1);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (Entity.Parent != null)
            {
                Position = Entity.Parent.Transform.Position + Position;
            }
        }

        public void Translate(Vector3 translation)
        {
            Translate(translation.X, translation.Y, translation.Z);
        }

        public void Translate(float x, float y, float z)
        {
            TranslateX(x);
            TranslateY(y);
            TranslateZ(z);
        }

        public void TranslateX(float translation)
        {
            Position = new Vector3(Position.X + translation, Position.Y, Position.Z);
        }

        public void TranslateY(float translation)
        {
            Position = new Vector3(Position.X, Position.Y + translation, Position.Z);
        }

        public void TranslateZ(float translation)
        {
            Position = new Vector3(Position.X, Position.Y, Position.Z + translation);
        }

        public void Rotate(Vector3 angles)
        {
            Rotate(angles.X, angles.Y, angles.Z);
        }

        public void Rotate(float x, float y, float z)
        {
            RotateX(x);
            RotateY(y);
            RotateZ(z);
        }

        public void RotateX(float angle)
        {
            float angleRad = angle * MathF.PI / 180.0f;
            float halfAngle = angleRad / 2;

            Rotation = new Quaternion(MathF.Sin(halfAngle), 0, 0, MathF.Cos(halfAngle)) * Rotation;
        }

        public void RotateY(float angle)
        {
            float angleRad = angle * MathF.PI / 180.0f;
            float halfAngle = angleRad / 2;

            Rotation = new Quaternion(0, MathF.Sin(halfAngle), 0, MathF.Cos(halfAngle)) * Rotation;
        }

        public void RotateZ(float angle)
        {
            float angleRad = angle * MathF.PI / 180.0f;
            float halfAngle = angleRad / 2;

            Rotation = new Quaternion(0, 0, MathF.Sin(halfAngle), MathF.Cos(halfAngle)) * Rotation;
        }

    }
}
