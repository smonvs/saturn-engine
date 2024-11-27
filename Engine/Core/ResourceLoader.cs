using SaturnEngine.Engine.Structs;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

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
                    default:
                        Log.Error($"ResourceLoader: File at filepath \"{filepath}\" could not be loaded. Reason: Filetype \"{fileExtension}\" not supported");
                        return null;
                }

                _resources.Add(resource);
                Log.Info($"ResourceLoader: File at filepath \"{filepath}\" was loaded");
                return TryReturnFile<T>(resource);
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
                    case "f":
                        triangles.Add(new Triangle(
                            vertices[int.Parse(parts[1].Split('/')[0]) - 1],
                            vertices[int.Parse(parts[2].Split('/')[0]) - 1],
                            vertices[int.Parse(parts[3].Split('/')[0]) - 1]
                        ));
                        break;
                }
            }

            return new Mesh(triangles.ToArray(), filepath);
        }

    }
}
