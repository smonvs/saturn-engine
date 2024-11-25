using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace SaturnEngine.Engine.Core
{
    internal class Window
    {

        private Application _application;
        private string _title;
        private Vector2 _size;

        private IntPtr _sdlWindow;
        private IntPtr _sdlRenderer;
        private SDL.SDL_Event _sdlEvent;

        public static Vector2 Size { get; private set; }

        internal Window(Application application, string title, int width, int height)
        {
            _size = new Vector2(width, height);
            _title = title;
            Window.Size = _size;
        }

        internal bool Open()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Log.Error($"SDL_ERROR: {SDL.SDL_GetError()}");
                return false;
            }

            // Create window
            _sdlWindow = SDL.SDL_CreateWindow
            (
                _title,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                (int)_size.X,
                (int)_size.Y,
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
            );

            if(_sdlWindow == IntPtr.Zero)
            {
                Log.Error($"SDL_ERROR: {SDL.SDL_GetError()}");
                SDL.SDL_Quit();
                return false;
            }

            // Create renderer
            _sdlRenderer = SDL.SDL_CreateRenderer(_sdlWindow, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            if(_sdlRenderer == IntPtr.Zero)
            {
                Log.Error($"SDL_ERROR: {SDL.SDL_GetError()}");
                SDL.SDL_Quit();
                SDL.SDL_DestroyWindow(_sdlWindow);
                return false;
            }

            return true;
        }

        internal IntPtr GetRenderer()
        {
            return _sdlRenderer;
        }

        internal void ProcessMessages()
        {
            while(SDL.SDL_PollEvent(out _sdlEvent) != 0)
            {
                switch (_sdlEvent.type)
                {
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        Input.KeyDown((KeyCode)_sdlEvent.key.keysym.sym);
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                        Input.KeyUp((KeyCode)_sdlEvent.key.keysym.sym);
                        break;
                    case SDL.SDL_EventType.SDL_QUIT:
                        _application.Running = false;
                        break;
                }
            }
        }

        internal void Draw()
        {
            SDL.SDL_RenderPresent(_sdlRenderer);
            SDL.SDL_SetRenderDrawColor(_sdlRenderer, 0x00, 0x00, 0x00, 0xFF);
            SDL.SDL_RenderClear(_sdlRenderer);
        }

    }
}
