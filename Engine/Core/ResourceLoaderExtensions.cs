using SaturnEngine.Engine.Components;

namespace SaturnEngine.Engine.Core
{
    public static class ResourceLoaderExtensions
    {

        public static T? LoadResource<T>(this ComponentBase component, string filepath) where T : Resource
        {
            return ResourceLoader.Load<T>(component.Entity.Scene, filepath);
        } 
        
    }
}
