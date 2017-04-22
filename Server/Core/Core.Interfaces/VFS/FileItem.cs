using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.FS
{
    /// <summary>
    /// Element of the file system : file
    /// </summary>
    public class FileItem :FsItem
    {
        public FileItem(string name)
            : base(name)
        { 
        }

        public FileItem(string name, string path)
            : base(name, path)
        {
        }
    }
}
