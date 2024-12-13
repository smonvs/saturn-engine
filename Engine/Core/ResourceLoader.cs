using SaturnEngine.Engine.Structs;
using System.Globalization;
using System.IO.Compression;
using System.Xml.Linq;
using System.Drawing;
using System.IO;
using SaturnEngine.Engine.Components;

namespace SaturnEngine.Engine.Core
{
    internal static class ResourceLoader
    {

        private static Dictionary<Scene, ResourceCollection> _resources = new Dictionary<Scene, ResourceCollection>();

        internal static T? Load<T>(Scene scene, string filepath) where T : Resource
        {
            filepath = filepath.Replace("\\", "/").ToLower();

            if (!File.Exists(filepath))
            {
                Log.Error($"ResourceLoader: File at filepath \"{filepath}\" could not be loaded. Reason: File not found");
                return null;
            }

            Resource? resource = null;

            Scene? found = null;
            foreach(var (keyScene, collection) in _resources)
            {
                if (collection.Contains(filepath))
                {
                    found = keyScene;
                    break;
                } 
            }

            if(found != null)
            {
                resource = (T)_resources[found][filepath];
                if (found != scene)
                {
                    if (!_resources.ContainsKey(scene)) _resources.Add(scene, new ResourceCollection());
                    if (!_resources[scene].Contains(resource)) _resources[scene].Add(resource);
                }
            }
            else
            {
                string fileExtension = Path.GetExtension(filepath);

                switch (fileExtension)
                {
                    case ".obj":
                        resource = LoadObj(filepath);
                        break;
                    case ".png":
                        resource = LoadPNG(filepath);
                        break;
                    default:
                        Log.Error($"ResourceLoader: File at filepath \"{filepath}\" could not be loaded. Reason: Filetype \"{fileExtension}\" not supported");
                        return null;
                }

                if (resource != null)
                {
                    if (!_resources.ContainsKey(scene)) _resources.Add(scene, new ResourceCollection());
                    _resources[scene].Add(resource);
                }
                else
                {
                    Log.Error($"ResourceLoader: File at filepath \"{filepath}\" could not be loaded. Reason: Unknown");
                    return null;
                }
            }

            return TryReturnFile<T>(resource);
        }

        internal static Scene LoadScene(string filepath)
        {
            filepath = filepath.Replace("\\", "/").ToLower();

            Dictionary<uint, string> resourcePaths = new Dictionary<uint, string>();
            Dictionary<uint, uint> entitiesRelations = new Dictionary<uint, uint>(); // key = Parent, value = Child)

            if (File.Exists(filepath))
            {
                try
                {
                    XDocument doc = XDocument.Load(filepath);

                    XElement scene = doc.Element("Scene");
                    if(scene != null)
                    {
                        Scene result = SceneManager.CreateScene(scene.Attribute("name")?.Value);

                        IEnumerable<XElement> resources = scene.Element("Resources")?.Elements("Resource");
                        if(resources != null)
                        {
                            foreach (XElement resource in resources)
                            {
                                resourcePaths.Add(uint.Parse(resource.Attribute("id")?.Value), resource.Value);
                            }
                        }

                        IEnumerable<XElement> entities = scene.Element("Entities")?.Elements("Entity");
                        if(entities != null)
                        {
                            Entity newEntity;

                            foreach (XElement entity in entities)
                            {
                                newEntity = Entity.Create(uint.Parse(entity.Attribute("id").Value), entity.Attribute("name")?.Value);
                            
                                if (entity.Attribute("isEnabled").Value == "false") newEntity.Disable();
                                if (entity.Attribute("parent").Value != "") entitiesRelations.Add(uint.Parse(entity.Attribute("parent").Value), newEntity.Id);
                                result.AddEntity(newEntity);

                                IEnumerable<XElement> components = entity.Elements("Component");
                                if(components != null)
                                {
                                    foreach(XElement component in components)
                                    {
                                        string typeName = component.Attribute("type")?.Value;
                                        Type type = Type.GetType($"SaturnEngine.Engine.Components.{typeName}, SaturnEngine");
                                        
                                        try
                                        {
                                            ComponentBase newComponent = (ComponentBase)typeof(Entity).GetMethod("AddComponent").MakeGenericMethod(type).Invoke(newEntity, null);

                                            IEnumerable<XElement> properties = component.Elements("Property");
                                            if(properties != null)
                                            {
                                                foreach (XElement property in properties)
                                                {
                                                    string propertyName = property.Attribute("name")?.Value;
                                                    string propertyType = property.Attribute("type")?.Value;

                                                    if (property.HasElements)
                                                    {
                                                        object complexObject = Activator.CreateInstance(type.GetProperty(propertyName).PropertyType);
                                                        SetProperties(complexObject, property.Elements("Property"));
                                                        type.GetProperty(propertyName).SetValue(newComponent, complexObject);
                                                    }
                                                    else if (property.Attribute("extern") != null)
                                                    {
                                                        uint resourceId = uint.Parse(property.Attribute("extern")?.Value);
                                                        string resourcePath = resourcePaths[resourceId];
                                                        Resource resource = Load<Resource>(result, resourcePath);
                                                        type.GetProperty(propertyName).SetValue(newComponent, resource);
                                                    }
                                                    else if (property.Value.ToLower() == "null")
                                                    {
                                                        type.GetProperty(propertyName).SetValue(newComponent, null);
                                                    }
                                                    else
                                                    {
                                                        switch (propertyType)
                                                        {
                                                            case "":
                                                                throw new Exception($"Property type \"{propertyType}\" unknown");
                                                            case "string":
                                                                string stringValue = property.Value;
                                                                type.GetProperty(propertyName).SetValue(newComponent, stringValue);
                                                                break;
                                                            case "int":
                                                                int intValue = int.Parse(property.Value, CultureInfo.InvariantCulture);
                                                                type.GetProperty(propertyName).SetValue(newComponent, intValue);
                                                                break;
                                                            case "uint":
                                                                uint uintValue = uint.Parse(property.Value, CultureInfo.InvariantCulture);
                                                                type.GetProperty(propertyName).SetValue(newComponent, uintValue);
                                                                break;
                                                            case "long":
                                                                long longValue = long.Parse(property.Value, CultureInfo.InvariantCulture);
                                                                type.GetProperty(propertyName).SetValue(newComponent, longValue);
                                                                break;
                                                            case "ulong":
                                                                ulong ulongValue = ulong.Parse(property.Value, CultureInfo.InvariantCulture);
                                                                type.GetProperty(propertyName).SetValue(newComponent, ulongValue);
                                                                break;
                                                            case "float":
                                                                float floatValue = float.Parse(property.Value, CultureInfo.InvariantCulture);
                                                                type.GetProperty(propertyName).SetValue(newComponent, floatValue);
                                                                break;
                                                            case "double":
                                                                double doubleValue = double.Parse(property.Value, CultureInfo.InvariantCulture);
                                                                type.GetProperty(propertyName).SetValue(newComponent, doubleValue);
                                                                break;
                                                            case "bool":
                                                                bool boolValue = bool.Parse(property.Value);
                                                                type.GetProperty(propertyName).SetValue(newComponent, boolValue);
                                                                break;
                                                            default: // assuming it's an enum
                                                                object enumValue = Enum.Parse(type.GetProperty(propertyName).PropertyType, property.Value);
                                                                type.GetProperty(propertyName).SetValue(newComponent, enumValue);
                                                                break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception($"Component could not be parsed. Reason: {e.Message}");
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Entity had no components");
                                }

                            }
                        }

                        foreach(var (parentId, childId) in entitiesRelations)
                        {
                            result.FindEntity(parentId).AddChild(result.FindEntity(childId));
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

        private static void SetProperties(object obj, IEnumerable<XElement> properties)
        {
            foreach (XElement property in properties)
            {
                string propertyName = property.Attribute("name")?.Value;
                string propertyType = property.Attribute("type")?.Value;

                if (property.HasElements)
                {
                    object complexObject = Activator.CreateInstance(obj.GetType().GetProperty(propertyName).PropertyType);
                    SetProperties(complexObject, property.Elements("Property"));
                    obj.GetType().GetProperty(propertyName).SetValue(obj, complexObject);
                }
                else
                {
                    switch (propertyType)
                    {
                        case "":
                            throw new Exception($"Property type \"{propertyType}\" unknown");
                        case "string":
                            string stringValue = property.Value;
                            obj.GetType().GetProperty(propertyName).SetValue(obj, stringValue);
                            break;
                        case "int":
                            int intValue = int.Parse(property.Value, CultureInfo.InvariantCulture);
                            obj.GetType().GetProperty(propertyName).SetValue(obj, intValue);
                            break;
                        case "uint":
                            uint uintValue = uint.Parse(property.Value, CultureInfo.InvariantCulture);
                            obj.GetType().GetProperty(propertyName).SetValue(obj, uintValue);
                            break;
                        case "float":
                            float floatValue = float.Parse(property.Value, CultureInfo.InvariantCulture);
                            obj.GetType().GetProperty(propertyName).SetValue(obj, floatValue);
                            break;
                        case "double":
                            double doubleValue = double.Parse(property.Value, CultureInfo.InvariantCulture);
                            obj.GetType().GetProperty(propertyName).SetValue(obj, doubleValue);
                            break;
                        case "bool":
                            bool boolValue = bool.Parse(property.Value);
                            obj.GetType().GetProperty(propertyName).SetValue(obj, boolValue);
                            break;
                        case "long":
                            long longValue = long.Parse(property.Value, CultureInfo.InvariantCulture);
                            obj.GetType().GetProperty(propertyName).SetValue(obj, longValue);
                            break;
                        case "ulong":
                            ulong ulongValue = ulong.Parse(property.Value, CultureInfo.InvariantCulture);
                            obj.GetType().GetProperty(propertyName).SetValue(obj, ulongValue);
                            break;
                        default: // assuming it's an enum
                            object enumValue = Enum.Parse(obj.GetType().GetProperty(propertyName).PropertyType, property.Value);
                            obj.GetType().GetProperty(propertyName).SetValue(obj, enumValue);
                            break;
                    }
                }
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

        private static Mesh LoadObj(string filepath)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> texCoords = new List<Vector2>();
            List<Triangle> triangles = new List<Triangle>();

            using(StreamReader streamReader = new StreamReader(filepath))
            {
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
