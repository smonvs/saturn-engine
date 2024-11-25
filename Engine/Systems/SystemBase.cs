using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaturnEngine.Engine.Core;

namespace SaturnEngine.Engine.Systems
{
    internal abstract class SystemBase<T>
    {

        protected Dictionary<Entity, T> _cache = new Dictionary<Entity, T>();

        protected internal abstract void Update();

        internal void AddToCache(Entity entity, T component)
        {
            _cache.Add(entity, component);
        }

        internal void RemoveFromCache(Entity entity)
        {
            _cache.Remove(entity);
        }

    }
}
