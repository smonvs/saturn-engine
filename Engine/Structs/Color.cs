using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static readonly Color Orange = new Color(0xFF, 0xAA, 0x00, 0xFF);

        #endregion

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = 0xFF;
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

    }
}
