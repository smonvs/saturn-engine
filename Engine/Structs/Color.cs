using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace SaturnEngine.Engine.Structs
{
    public struct Color
    {

        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public byte A { get; }

        #region Static Colors

        public static readonly Color White = new Color(0xFF, 0xFF, 0xFF, 0xFF);
        public static readonly Color Black = new Color(0x00, 0x00, 0x00, 0xFF);
        public static readonly Color Red = new Color(0xFF, 0x00, 0x00, 0xFF);
        public static readonly Color Green = new Color(0x00, 0xFF, 0x00, 0xFF);
        public static readonly Color Blue = new Color(0x00, 0x00, 0xFF, 0xFF);
        public static readonly Color Yellow = new Color(0xFF, 0xFF, 0x00, 0xFF);
        public static readonly Color Cyan = new Color(0x00, 0xFF, 0xFF, 0xFF);
        public static readonly Color Magenta = new Color(0xFF, 0x00, 0xFF, 0xFF);
        public static readonly Color Gray = new Color(0x80, 0x80, 0x80, 0xFF);
        public static readonly Color LightGray = new Color(0xD3, 0xD3, 0xD3, 0xFF);
        public static readonly Color DarkGray = new Color(0xA9, 0xA9, 0xA9, 0xFF);
        public static readonly Color Orange = new Color(0xFF, 0xA5, 0x00, 0xFF);
        public static readonly Color Purple = new Color(0x80, 0x00, 0x80, 0xFF);
        public static readonly Color Brown = new Color(0xA5, 0x2A, 0x2A, 0xFF);
        public static readonly Color Pink = new Color(0xFF, 0xC0, 0xCB, 0xFF);

        #endregion

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public SDL.SDL_Color ToSDL_Color()
        {
            return new SDL.SDL_Color { r = R, g = G, b = B, a = A };
        }   

    }
}
