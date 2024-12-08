using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SaturnEngine.Engine.Core
{
    public static class Input
    {

        public static Vector2 MousePosition { get; private set; } = Vector2.Zero;

        private static Dictionary<KeyCode, bool> _keys = new Dictionary<KeyCode, bool>()
        {
            
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
