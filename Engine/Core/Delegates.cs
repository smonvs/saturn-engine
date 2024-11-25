using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaturnEngine.Engine.Components;

namespace SaturnEngine.Engine.Core
{

    public delegate void OnEntityAdded(Entity entity);
    public delegate void OnEntityDestroyed(Entity entity);
    public delegate void OnComponentAdded(Entity entity, ComponentBase component);

}
