using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.FsExceptions;

namespace Core.FS
{
    /// <summary>
    /// Directory in file system
    /// </summary>
    public class DirectoryItem : FsItem
    {
        /// <summary>
        /// Create new directory item descriptor
        /// </summary>
        /// <param name="name">Name of the folder</param>
        public DirectoryItem(string name) : base(name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ParamsNotAllowedException("Directory name connot be empty!");
        }

        /// <summary>
        /// Create new directory item descriptor
        /// </summary>
        /// <param name="name">Name of the folder</param>
        /// <param name="path">Desired path</param>
        public DirectoryItem(string name, string path)
            : base(name, path)
        {

        }

    }
}

