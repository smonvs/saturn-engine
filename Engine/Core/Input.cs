using SaturnEngine.Engine.Structs;

namespace SaturnEngine.Engine.Core
{
    public static class Input
    {

        public static Vector2 MousePosition { get; internal set; } = Vector2.Zero;
        public static Vector2 MouseDirection 
        { 
            get 
            { 
                if(MousePosition == _mousePositionPrev)
                {
                    return Vector2.Zero;
                }
                else
                {
                    return (MousePosition - _mousePositionPrev).Normalize();
                }
            } 
        }

        private static Dictionary<KeyCode, bool> _keys = new Dictionary<KeyCode, bool>()
        {            { KeyCode.Mouse1, false },
            { KeyCode.Mouse2, false },
            { KeyCode.Mouse3, false },
            { KeyCode.Back, false },
            { KeyCode.Tab, false },
            { KeyCode.Return, false },
            { KeyCode.Shift, false },
            { KeyCode.Control, false },
            { KeyCode.Escape, false },
            { KeyCode.Space, false },
            { KeyCode.PageUp, false },
            { KeyCode.PageDown, false },
            { KeyCode.End, false },
            { KeyCode.Home, false },
            { KeyCode.ArrowLeft, false },
            { KeyCode.ArrowUp, false },
            { KeyCode.ArrowRight, false },
            { KeyCode.ArrowDown, false },
            { KeyCode.Insert, false },
            { KeyCode.Delete, false },
            { KeyCode.Key0, false },
            { KeyCode.Key1, false },
            { KeyCode.Key2, false },
            { KeyCode.Key3, false },
            { KeyCode.Key4, false },
            { KeyCode.Key5, false },
            { KeyCode.Key6, false },
            { KeyCode.Key7, false },
            { KeyCode.Key8, false },
            { KeyCode.Key9, false },
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
            { KeyCode.F1, false },
            { KeyCode.F2, false },
            { KeyCode.F3, false },
            { KeyCode.F4, false },
            { KeyCode.F5, false },
            { KeyCode.F6, false },
            { KeyCode.F7, false },
            { KeyCode.F8, false },
            { KeyCode.F9, false },
            { KeyCode.F10, false },
            { KeyCode.F11, false },
            { KeyCode.F12, false }
        };
        private static Dictionary<KeyCode, bool> _prevKeys = new Dictionary<KeyCode, bool>(_keys);
        private static Vector2 _mousePositionPrev = Vector2.Zero;

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
            _mousePositionPrev = MousePosition;
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
