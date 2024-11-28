using SaturnEngine.Engine.Components;
using SaturnEngine.Engine.Core;
using SaturnEngine.Engine.Structs;

using SDL_Renderer = nint;

namespace SaturnEngine.Engine.Systems
{
    internal class Renderer3DSystem : SystemBase<Renderer3D>
    {

        private SDL_Renderer _renderer;

        internal Renderer3DSystem(SDL_Renderer renderer)
        {
            _renderer = renderer;
        }

        protected internal override void Update()
        {
            Camera3D camera = Camera3D.Main;

            if(camera != null)
            {
                Matrix4x4 projectionMatrix = Matrix4x4.MakeProjection(camera.FieldOfView, Window.Size.Y / Window.Size.X, 0.1f, 1000.0f);

                foreach (Renderer3D component in _cache.Values)
                {
                    if (component.IsEnabled) component.OnRender(_renderer, projectionMatrix, camera);
                }
            }
        }

    }
}
