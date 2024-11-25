using System;
using SaturnEngine.Engine.Core;

namespace SaturnEngine.Engine.Components
{
    public abstract class ComponentBase
    {

        public Entity Entity { get; internal set; }
        public Transform Transform { get { return Entity.Transform; } }

        private bool _isEnabled = true;
        public bool IsEnabled { get { return _isEnabled && Entity.IsEnabled; } }

        public virtual void OnInit() { }

        public virtual void OnStart() { }

        public virtual void OnUpdate(float deltaTime) { }

        public virtual void OnDestroy() { }

        public virtual void OnEnable() { }

        public virtual void OnDisable() { }

        public void Enable()
        {
            _isEnabled = true;
            OnEnable();
        }

        public void Disable()
        {
            _isEnabled = false;
            OnDisable();
        }

        public void Destroy()
        {
            OnDestroy();
        }

    }
}
