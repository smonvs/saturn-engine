using System;
using SaturnEngine.Engine.Components;

namespace SaturnEngine.Engine.Core
{
    public class Entity
    {

        private static uint _lastId = 0;

        private bool _isEnabled;
        public bool IsEnabled { get { return _isEnabled && (Parent?.IsEnabled ?? true); } }

        public uint Id { get; private set; }
        public string Name { get; set; }
        public Scene Scene { get; private set; }
        public Entity Parent { get; private set; }
        public Transform Transform { get; private set; }

        private Dictionary<Type, ComponentBase> _components = new Dictionary<Type, ComponentBase>();
        private EntityCollection _children = new EntityCollection();

        public event OnEntityDestroyed OnEntityDestroyed;
        public event OnComponentAdded OnComponentAdded;

        private Entity() { }

        #region Operators

        public static bool operator ==(Entity a, Entity b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;

            return a.Id == b.Id;
        }   

        public static bool operator!=(Entity a, Entity b)
        {
            return !(a == b);
        }

        #endregion

        public static Entity Create(string name)
        {
            return Create(_lastId++, name);
        }

        internal static Entity Create(uint id, string name)
        { 
            Entity entity = new Entity();
            entity._isEnabled = true;
            entity.Id = id;
            entity.Name = name;

            Log.Info(entity, "Entity was created");

            entity.Transform = entity.AddComponent<Transform>();

            _lastId = id;

            return entity;
        }

        public void Enable()
        {
            _isEnabled = true;
        }

        public void Disable()
        {
            _isEnabled = false;
        }

        public void Destroy()
        {
            OnEntityDestroyed?.Invoke(this); 
            Log.Info(this, "Entity was destroyed");
        }

        internal void OnAddedToScene(Scene scene)
        {
            Scene = scene;

            Log.Info(this, $"Entity was added to scene \"{scene.Name}\"");

            foreach (ComponentBase component in _components.Values)
            {
                component.OnStart();
            }
        }

        #region Component Methods

        public void UpdateComponents(float deltaTime)
        {
            foreach(ComponentBase component in _components.Values)
            {
                if (component.IsEnabled) component.OnUpdate(deltaTime);
            }
        }

        public T AddComponent<T>() where T : ComponentBase, new()
        {
            if (!HasComponent<T>())
            { 
                Type type = typeof(T);

                T component = new T();
                component.Entity = this;
                _components.Add(type, component);
                component.OnInit();

                OnComponentAdded?.Invoke(this, component);

                Log.Info(this, $"Component \"{type}\" was added");

                if (Scene != null) component.OnStart();
            }

            return GetComponent<T>();
        }

        public T GetComponent<T>() where T : ComponentBase
        {
            Type type = typeof(T);

            if (_components.ContainsKey(type)) 
            {
                return (T)_components[type];
            }
            else
            {
                Log.Warning(this, $"Component \"{type}\" not found");
                return null;
            }
        }

        public ComponentBase[] GetComponents()
        {
            return _components.Values.ToArray();
        }

        public bool HasComponent<T>() where T : ComponentBase
        {
            return _components.ContainsKey(typeof(T));
        }

        #endregion

        #region Hierarchy Methods

        public void AddChild(Entity entity)
        {
            _children.Add(entity);

            if(entity.Parent != null)
            {
                entity.Parent.RemoveChild(entity);
            }

            entity.Parent = this;
        
            Log.Info(this, $"Entity \"{entity.Name}\" was added as child");
        }

        public void RemoveChild(Entity entity)
        {
            RemoveChild(entity.Id);
        }

        public void RemoveChild(int index)
        {
            if (index > 0 && index < _children.Count)
            {
                Entity child = _children.ElementAt(index);
                child.Parent = null;
                _children.Remove(child.Id);
                Log.Info(this, $"Entity \"{child.Name}\" was removed from children");
            }
            else
            {
                Log.Warning(this, $"Can't remove child at index '{index}'. Reason: Index out of bounds");
            }
        }

        public void RemoveChild(uint id)
        {
            if (_children.Contains(id))
            {
                Entity child = _children[id];
                child.Parent = null;
                _children.Remove(id);
                Log.Info(this, $"Entity \"{child.Name}\" was removed from children");
            }
            else
            {
                Log.Warning(this, $"Can't remove child with ID '{id}'. Reason: Not found");
            }
        }

        public Entity[] GetChildren()
        {
            return _children.ToArray();
        }

        #endregion

    }
}
