using SaturnEngine.Engine.Core;

namespace SaturnEngine.Engine.Components
{
    public class Camera3D : ComponentBase
    {

        public static Camera3D Main { get; internal set; }

        [Export] public float FieldOfView { get; set; } = 90.0f;

        public override void OnStart()
        {
            if (IsEnabled)
            {
                OnEnable();
            }
        }

        public override void OnEnable()
        {
            if (Main != null) Main.Disable();
            Main = this;
        }

    }
}
