using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SaturnEngine.Engine.Core
{
    public class EntityCollection : KeyedCollection<uint, Entity>
    {

        protected override uint GetKeyForItem(Entity item) => item.Id;

    }
}
