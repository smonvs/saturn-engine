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

        private static Mesh LoadObj(string filepath, StreamReader streamReader)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> texCoords = new List<Vector2>();
            List<Triangle> triangles = new List<Triangle>();

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                string[] parts = line.Split(' ');

                switch (parts[0])
                {
                    case "v":
                        vertices.Add(new Vector3(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)
                        ));
                        break;
                    case "vt":
                        texCoords.Add(new Vector2(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture)
                        ));
                        break;
                    case "f":
                        string[] vertex1 = parts[1].Split('/');
                        string[] vertex2 = parts[2].Split('/');
                        string[] vertex3 = parts[3].Split('/');

                        if (vertex1.Length > 1)
                        {
                            triangles.Add(new Triangle(
                                vertices[int.Parse(vertex1[0]) - 1],
                                vertices[int.Parse(vertex2[0]) - 1],
                                vertices[int.Parse(vertex3[0]) - 1],
                                texCoords[int.Parse(vertex1[1]) - 1],
                                texCoords[int.Parse(vertex2[1]) - 1],
                                texCoords[int.Parse(vertex3[1]) - 1]
                            ));
                        }
                        else
                        {
                            triangles.Add(new Triangle(
                                vertices[int.Parse(vertex1[0]) - 1],
                                vertices[int.Parse(vertex2[0]) - 1],
                                vertices[int.Parse(vertex3[0]) - 1]
                            ));
                        }
                        break;
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

        }

    }
}
