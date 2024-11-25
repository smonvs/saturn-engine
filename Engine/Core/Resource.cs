using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaturnEngine.Engine.Core
{
    public abstract class Resource
    {

        public string? Filepath { get; }
        public string? Filename { get; }
        public string? FileExtension { get; }

        public Resource(string? filepath)
        {
            if(filepath != null)
            {
                Filepath = filepath;
                Filename = Path.GetFileNameWithoutExtension(filepath);
                FileExtension = Path.GetExtension(filepath);
            }
            else
            {
                Filepath = null; 
                Filename = null; 
                FileExtension = null;
            }
        }

    }
}
