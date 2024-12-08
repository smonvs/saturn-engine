using SaturnEngine.Engine.Structs;

namespace SaturnEngine.Engine.Core
{
    public class Texture : Resource
    {

        public int Width { get; }
        public int Height { get; }
        public byte[] Pixels { get; }

        public Texture(int width, int height, byte[] pixels, string filepath) : base(filepath)
        {
            Width = width;
            Height = height;
            Pixels = pixels;
        }

        public Color GetPixelColor(int x, int y)
        {
            int index = (y * Width + x) * 4;
            byte r = Pixels[index];
            byte g = Pixels[index + 1];
            byte b = Pixels[index + 2];
            byte a = Pixels[index + 3];

            return new Color(r, g, b, a);
        }

    }
}
