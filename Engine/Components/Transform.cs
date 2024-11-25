using SaturnEngine.Engine.Core;
using SaturnEngine.Engine.Structs;
using System;

namespace SaturnEngine.Engine.Components
{
    public class Transform : ComponentBase
    {

        public Vector3 Position { get; set; }
        public System.Numerics.Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public override void OnInit()
        {
            Position = Vector3.Zero;
            Rotation = System.Numerics.Quaternion.Identity;
            Scale = new Vector3(1, 1, 1);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (Entity.Parent != null)
            {
                Position = Entity.Parent.Transform.Position + Position;
            }
        }

    }
}
