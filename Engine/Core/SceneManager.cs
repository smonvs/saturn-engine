using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaturnEngine.Engine.Core
{
    public static class SceneManager
    {

        private static Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();

        public static event OnEntityDestroyed OnEntityDestroyed;
        public static event OnComponentAdded OnComponentAdded;

        internal static Scene CreateScene(string name)
        {
            Scene scene = new Scene(name);
            _scenes.Add(name, scene);
            scene.OnComponentAdded += OnComponentAdded;
            scene.OnEntityDestroyed += OnEntityDestroyed;
            return scene;
        }

        internal static void UpdateScenes(float deltaTime)
        {
            foreach (Scene scene in _scenes.Values)
            {
                scene.UpdateEntities(deltaTime);
            }
        }

        public static bool LoadScene(string filepath)
        {
            Scene scene = ResourceLoader.LoadScene(filepath);
            
            if(scene != null)
            {
                _scenes[scene.Name] = scene;
                return true;
            }
            else
            {
                return false;
            }
        }



        public static bool UnloadScene(string name)
        {
            return true;
        }

    }
}
