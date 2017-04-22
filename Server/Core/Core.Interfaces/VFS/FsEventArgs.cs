using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Sessions;

namespace Core.FS
{
    /// <summary>
    /// Event argument: command execution completed
    /// </summary>
    public class FsEventArgs : EventArgs
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        public String CommandName { get; set; }

        /// <summary>
        /// User descriptor
        /// </summary>
        public SID UserToken { get; set; }
    }
}
