using SaturnEngine.Engine.Structs;

using SDL_Renderer = nint;

namespace SaturnEngine.Engine.Components
{
    public abstract class Renderer3D : ComponentBase
    {

        internal abstract void OnRender(SDL_Renderer renderer, Matrix4x4 projectionMatrix, Camera3D camera);

    }
}
