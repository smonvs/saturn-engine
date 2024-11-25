using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaturnEngine.Engine.Components;
using SDL2;

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
            foreach (Renderer3D component in _cache.Values)
            {
                if (component.IsEnabled) component.OnRender(_renderer);
            }
        }

    }
}
