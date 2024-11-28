using SaturnEngine.Engine.Components;
using SaturnEngine.Engine.Systems;
using System.Diagnostics;
using SDL2;
using System.Runtime.InteropServices;

namespace SaturnEngine.Engine.Core
{
    public class Application
    {

        #region Fields & Properties

        public bool Running { get; set; }

        private Window _window;
        private SceneManager _sceneManager;

        private Renderer3DSystem _renderer3DSystem;

        #endregion

        #region Events

        public delegate void OnApplicationLoad(SceneManager sceneManager);
        public delegate void OnApplicationClose();

        public event OnApplicationLoad OnLoad;
        public event OnApplicationClose OnClose;

        #endregion

        #region Release

        #if DEBUG
        #else
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        #endif

        #endregion

        public Application(string title, int width, int height)
        {
            _window = new Window(this, title, width, height);
            _sceneManager = new SceneManager();
            _sceneManager.OnEntityDestroyed += OnEntityDestroyed;
            _sceneManager.OnComponentAdded += OnComponentAdded;
            
            // Hide Console window if not in debug mode
            #if DEBUG
            #else
            ShowWindow(GetConsoleWindow(), 0);
            #endif
        }

        public void Run()
        {
            Running = _window.Open();

            _renderer3DSystem = new Renderer3DSystem(_window.GetRenderer());

            OnLoad?.Invoke(_sceneManager);

            Stopwatch stopwatch = new Stopwatch();
            float deltaTime = 0.0f;
            float elapsedTime = 0.0f;
            int frameCount = 0;

            while (Running)
            {
                stopwatch.Start();

                Input.Update();
                _window.ProcessMessages();

                _sceneManager.UpdateScenes(deltaTime);

                _window.Clear();

                _renderer3DSystem.Update();

                _window.Draw();

                //SDL.SDL_Delay(1000 / 144);

                stopwatch.Stop();
                deltaTime = stopwatch.ElapsedMilliseconds / 1000.0f;
                elapsedTime += deltaTime;
                frameCount++;
                stopwatch.Reset();

                if(elapsedTime >= 1.0f)
                {
                    _window.ShowFpsInTitle((int)(frameCount / elapsedTime), (int)(deltaTime * 1000));
                    frameCount = 0;
                    elapsedTime -= 1.0f;
                }
            }

            OnClose?.Invoke();
        }

        private void OnEntityDestroyed(Entity entity)
        {
            foreach (ComponentBase component in entity.GetComponents())
            {
                if (component is Renderer3D)
                {
                    _renderer3DSystem.RemoveFromCache(entity);
                }
            }
        }
        
        private void OnComponentAdded(Entity entity, ComponentBase component)
        {
            if(component is Renderer3D renderer3D)
            {
                _renderer3DSystem.AddToCache(entity, renderer3D);
            }
        }

    }
}
