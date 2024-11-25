using SaturnEngine.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaturnEngine.Engine.Core
{
    public class Scene
    {

        public string Name { get; private set; }

        private EntityCollection _entities = new EntityCollection();
        private List<Entity> _addedCache = new List<Entity>();
        private List<Entity> _destroyedCache = new List<Entity>();

        public event OnEntityDestroyed OnEntityDestroyed;
        public event OnComponentAdded OnComponentAdded;

        internal Scene(string name)
        {
            Name = name;
        }

        public void AddEntity(Entity entity)
        {
            _addedCache.Add(entity);
        }

        private void AddEntityFromCache(Entity entity)
        {
            _entities.Add(entity);
            entity.OnAddedToScene(this);
            entity.OnEntityDestroyed += OnEntityDestroyed;
            entity.OnComponentAdded += OnComponentAdded;

            foreach (ComponentBase component in entity.GetComponents())
            {
                OnComponentAdded?.Invoke(entity, component);
            }
        }

        internal void UpdateEntities(float deltaTime)
        {
            foreach(Entity entity in _entities) 
            { 
                if(entity.IsEnabled) entity.UpdateComponents(deltaTime);
            }

            foreach(Entity entity in _addedCache)
            {
                AddEntityFromCache(entity);

            }
            _addedCache.Clear();

            foreach(Entity entity in _destroyedCache)
            {
                _entities.Remove(entity);
                OnEntityDestroyed?.Invoke(entity);
            }
            _destroyedCache.Clear();

        }

    }
}
