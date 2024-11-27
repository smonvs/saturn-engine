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

        #endregion

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        #region operators

        public static bool operator==(Color c1, Color c2)
        {
            return c1.R == c2.R && c1.G == c2.G && c1.B == c2.B && c1.A == c2.A;
        }

        public static bool operator !=(Color c1, Color c2)
        {
            return !(c1 == c2);
        }

        #endregion

        public SDL.SDL_Color ToSDL_Color()
        {
            return new SDL.SDL_Color { r = R, g = G, b = B, a = A };
        }   

    }
}
