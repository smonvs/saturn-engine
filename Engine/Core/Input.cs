using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace SaturnEngine.Engine.Core
{
    public static class Input
    {

        public static Vector2 MousePosition { get; private set; } = Vector2.Zero;

        private static Dictionary<KeyCode, bool> _keys = new Dictionary<KeyCode, bool>()
        {
            { KeyCode.Return, false },
            { KeyCode.Escape, false },
            { KeyCode.Backspace, false },
            { KeyCode.Tab, false },
            { KeyCode.Space, false },
            { KeyCode._1, false },
            { KeyCode._2, false },
            { KeyCode._3, false },
            { KeyCode._4, false },
            { KeyCode._5, false },
            { KeyCode._6, false },
            { KeyCode._7, false },
            { KeyCode._8, false },
            { KeyCode._9, false },
            { KeyCode._0, false },
            { KeyCode.A, false },
            { KeyCode.B, false },
            { KeyCode.C, false },
            { KeyCode.D, false },
            { KeyCode.E, false },
            { KeyCode.F, false },
            { KeyCode.G, false },
            { KeyCode.H, false },
            { KeyCode.I, false },
            { KeyCode.J, false },
            { KeyCode.K, false },
            { KeyCode.L, false },
            { KeyCode.M, false },
            { KeyCode.N, false },
            { KeyCode.O, false },
            { KeyCode.P, false },
            { KeyCode.Q, false },
            { KeyCode.R, false },
            { KeyCode.S, false },
            { KeyCode.T, false },
            { KeyCode.U, false },
            { KeyCode.V, false },
            { KeyCode.W, false },
            { KeyCode.X, false },
            { KeyCode.Y, false },
            { KeyCode.Z, false },
            {KeyCode.ArrowUp, false },
            { KeyCode.ArrowDown, false },
            { KeyCode.ArrowLeft, false },
            { KeyCode.ArrowRight, false }
        };
        private static Dictionary<KeyCode, bool> _prevKeys = new Dictionary<KeyCode, bool>(_keys);

        internal static void KeyDown(KeyCode key)
        {
            _keys[key] = true;
        }

        internal static void KeyUp(KeyCode key)
        {
            _keys[key] = false;
        }

        internal static void Update()
        {
            int x, y;
            SDL.SDL_GetMouseState(out x, out y);

            MousePosition = new Vector2(x, y);
            _prevKeys = new Dictionary<KeyCode, bool>(_keys);
        }

        public static bool IsKeyPressed(KeyCode key)
        {
            return _keys[key];
        }

        public static bool IsKeyJustPressed(KeyCode key)
        {
            return _keys[key] && !_prevKeys[key];
        }

        public static bool IsKeyJustReleased(KeyCode key)
        {
            return !_keys[key] && _prevKeys[key];
        }
    
    }
}
