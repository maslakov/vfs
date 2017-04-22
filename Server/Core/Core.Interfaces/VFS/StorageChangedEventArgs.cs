using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Sessions;

namespace Core.FS
{
    /// <summary>
    /// Storage notification event argument
    /// </summary>
    public class StorageChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Source element
        /// </summary>
        public FsItem Item { get; set; }

        /// <summary>
        /// ID of author of the change
        /// </summary>
        public SID UserToken { get; set; }
    }
}
