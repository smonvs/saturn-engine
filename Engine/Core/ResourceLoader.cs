using SaturnEngine.Engine.Structs;
using System.Globalization;
using System.IO.Compression;
using System.Xml.Linq;
using System.Drawing;
using System.IO;

namespace SaturnEngine.Engine.Core
{
    public static class ResourceLoader
    {

        private static ResourceCollection _resources = new ResourceCollection();

        public static T? Load<T>(string filepath) where T : Resource
        {
            filepath = filepath.Replace("\\", "/").ToLower();

            if (_resources.Contains(filepath))
            {
                return TryReturnFile<T>(_resources[filepath]);
            }
            else if (!File.Exists(filepath))
            {
                Log.Error($"ResourceLoader: File at filepath \"{filepath}\" could not be loaded. Reason: File not found");
                return null;
            }
            else
            {
                string fileExtension = Path.GetExtension(filepath);
                Resource resource;

                switch (fileExtension)
                {
                    case ".obj":
                        using (StreamReader streamReader = new StreamReader(filepath))
                        {
                            resource = LoadObj(filepath, streamReader);
                        }
                        break;
                    case ".png":
                        resource = LoadPNG(filepath);
                        break;
                    default:
                        Log.Error($"ResourceLoader: File at filepath \"{filepath}\" could not be loaded. Reason: Filetype \"{fileExtension}\" not supported");
                        return null;
                }

                if(resource != null)
                {
                    _resources.Add(resource);
                    Log.Info($"ResourceLoader: File at filepath \"{filepath}\" was loaded");
                    return TryReturnFile<T>(resource);
                }

                return null;
            }
        }

        internal static Scene LoadScene(string filepath)
        {
            filepath = filepath.Replace("\\", "/").ToLower();

            Dictionary<uint, string> resourcePaths = new Dictionary<uint, string>();
            Dictionary<uint, Entity> entitiesTemp = new Dictionary<uint, Entity>();

            if (File.Exists(filepath))
            {
                try
                {
                    XDocument doc = XDocument.Load(filepath);

                    XElement scene = doc.Element("scene");
                    if(scene != null)
                    {
                        Scene result = new Scene(scene.Attribute("name")?.Value);

                        IEnumerable<XElement> resources = scene.Element("resource")?.Elements("Resource");
                        if(resources != null)
                        {
                            foreach (XElement resource in resources)
                            {
                                resourcePaths.Add(uint.Parse(resource.Attribute("id")?.Value), resource.Value);
                            }
                        }

                        IEnumerable<XElement> entities = scene.Element("entities")?.Elements("Entity");
                        if(entities != null)
                        {
                            Entity newEntity;

                            foreach (XElement entity in entities)
                            {
                                newEntity = Entity.Create(entity.Attribute("name")?.Value);
                                if(entity.Attribute("isEnabled")?.Value == "false") newEntity.Disable();

                                IEnumerable<XElement> components = entity.Elements("Component");
                                if(components != null)
                                {
                                    foreach(XElement component in components)
                                    {
                                        Type type = Type.GetType(component.Attribute("type")?.Value);



                                        try
                                        {

                                        }
                                        catch
                                        {
                                            throw new Exception("Component could not be parsed");
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Entity had no components");
                                }
                            }
                        }

                        return result;
                    }
                    else
                    {
                        throw new Exception("Scene could not be parsed");
                    }
                }
                catch(Exception e)
                {
                    Log.Error($"ResourceLoader: Scene at filepath \"{filepath}\" could not be loaded. Reason: Parsing error; {e.Message}");
                    return null;
                }
            }
            else
            {
                Log.Error($"ResourceLoader: Scene at filepath \"{filepath}\" could not be loaded. Reason: File not found");
                return null;
            }
        }

        private static T TryReturnFile<T>(Resource resource) where T : Resource
        {
            try
            {
                return (T)resource;
            }
            catch
            {
                Log.Error($"ResourceLoader: File at filepath \"{resource.Filepath}\" could not be loaded. Reason: Wrong filetype");
                return null;
            }
        }

        private static int ReadInt32(BinaryReader reader)
        {
            return BitConverter.ToInt32(reader.ReadBytes(4), 0);
        }

        private static byte[] Decompress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
            using (MemoryStream output = new MemoryStream())
            {
                ds.CopyTo(output);
                return output.ToArray();
            }
        }

        private static Mesh LoadObj(string filepath, StreamReader streamReader)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Triangle> triangles = new List<Triangle>();

            using (StreamReader reader = new StreamReader(filepath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("v "))
                    {
                        string[] parts = line.Split(' ');
                        vertices.Add(new Vector3(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)
                        ));
                    }
                    else if (line.StartsWith("vt "))
                    {
                        string[] parts = line.Split(' ');
                        uvs.Add(new Vector2(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture)
                        ));
                    }
                    else if (line.StartsWith("f "))
                    {
                        string[] parts = line.Split(' ');
                        int[] faceIndices = new int[parts.Length - 1];
                        int[] uvIndices = new int[parts.Length - 1];
                        for (int i = 1; i < parts.Length; i++)
                        {
                            string[] indices = parts[i].Split('/');
                            faceIndices[i - 1] = int.Parse(indices[0]) - 1;
                            if (indices.Length > 1 && indices[1] != "")
                            {
                                uvIndices[i - 1] = int.Parse(indices[1]) - 1;
                            }
                        }

                        // Triangulate the face (assuming it's a convex polygon)
                        for (int i = 1; i < faceIndices.Length - 1; i++)
                        {
                            triangles.Add(new Triangle(
                                vertices[faceIndices[0]], vertices[faceIndices[i]], vertices[faceIndices[i + 1]], 
                                uvs[uvIndices[0]], uvs[uvIndices[i]], uvs[uvIndices[i + 1]]
                            ));
                        }
                    }
                }
            }

            return new Mesh(triangles.ToArray(), filepath);
        }

        private static Texture LoadPNG(string filepath)
        {
            using(Bitmap bitmap = new Bitmap(filepath))
            {
                int width = bitmap.Width;
                int height = bitmap.Height;
                byte[] pixels = new byte[width * height * 4];

                for (int y = 0; y < height; y++)
                {
                    for(int x = 0; x < width; x++)
                    {
                        System.Drawing.Color color = bitmap.GetPixel(x, y);
                        
                        int index = (y * width + x) * 4;

                        pixels[index] = color.R;
                        pixels[index + 1] = color.G;
                        pixels[index + 2] = color.B;
                        pixels[index + 3] = color.A;
                    }
                }

                return new Texture(width, height, pixels, filepath);
            }

            Log.Error($"ResourceLoader: PNG at filepath \"{filepath}\" could not be loaded. Reason: PNG could not be parsed");
            return null;
        }

    }
}
