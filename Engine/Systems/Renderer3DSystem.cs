using SaturnEngine.Engine.Components;
using SaturnEngine.Engine.Core;
using SaturnEngine.Engine.Structs;

namespace SaturnEngine.Engine.Systems
{
    internal class Renderer3DSystem : SystemBase<Renderer3D>
    {

        private Window _window;
        private Matrix4x4 _projectionMatrix;

        internal Renderer3DSystem(Window window)
        {
            _window = window;
            CalculateProjectionMatrix();
        }

        private void CalculateProjectionMatrix()
        {

        }

        protected internal override void Update()
        {
            // TODO: move outside of update
            _projectionMatrix = Matrix4x4.MakeProjection(Camera3D.Main.FieldOfView, Window.BufferSize.Y / Window.BufferSize.X, 0.1f, 1000.0f);
            
            Camera3D camera = Camera3D.Main;

            if (camera != null)
            {
                foreach (Renderer3D component in _cache.Values)
                {
                    if (component.IsEnabled) component.OnRender(_window, _projectionMatrix, camera);
                }
            }
        }

    }
}
