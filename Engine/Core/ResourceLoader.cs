using SaturnEngine.Engine.Structs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        resource = LoadObj(filepath, new StreamReader(filepath));
                        break;
                    default:
                        Log.Error($"ResourceLoader: File at filepath \"{filepath}\" could not be loaded. Reason: Filetype \"{fileExtension}\" not supported");
                        return null;
                }

                _resources.Add(resource);
                return TryReturnFile<T>(resource);
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
