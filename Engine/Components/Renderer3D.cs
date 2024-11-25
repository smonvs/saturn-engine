using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaturnEngine.Engine.Components
{
    public abstract class Renderer3D : ComponentBase
    {

        internal abstract void OnRender(nint renderer);

    }
}
