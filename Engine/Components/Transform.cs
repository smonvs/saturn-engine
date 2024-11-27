using SaturnEngine.Engine.Core;
using SaturnEngine.Engine.Structs;
using System;

namespace SaturnEngine.Engine.Components
{
    public class Transform : ComponentBase
    {

        [Export] public Vector3 Position { get; set; }
        [Export] public Quaternion Rotation { get; set; }
        [Export] public Vector3 Scale { get; set; }

        public override void OnInit()
        {
            Position = Vector3.Zero;
            Rotation = new Quaternion();
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
