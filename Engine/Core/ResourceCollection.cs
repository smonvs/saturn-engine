using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaturnEngine.Engine.Core
{
    internal class ResourceCollection : KeyedCollection<string, Resource>
    {

        protected override string GetKeyForItem(Resource item)
        {
            if (item == null) return "";
            if(item.Filepath == null)
            {
                return "";
            }
            return item.Filepath;
        }

    }
}
