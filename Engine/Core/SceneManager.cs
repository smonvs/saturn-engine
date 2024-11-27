using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaturnEngine.Engine.Core
{
    public class SceneManager
    {

        private Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();

        public event OnEntityDestroyed OnEntityDestroyed;
        public event OnComponentAdded OnComponentAdded;

        public Scene CreateScene(string name)
        {
            Scene scene = new Scene(name);
            _scenes.Add(name, scene);
            scene.OnComponentAdded += OnComponentAdded;
            scene.OnEntityDestroyed += OnEntityDestroyed;
            return scene;
        }

        public void UpdateScenes(float deltaTime)
        {
            foreach (Scene scene in _scenes.Values)
            {
                scene.UpdateEntities(deltaTime);
            }
        }

        public bool LoadScene(string filepath)
        {
            return true;
        }



        public bool UnloadScene(string name)
        {
            return true;
        }

    }
}
