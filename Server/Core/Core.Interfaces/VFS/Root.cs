using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.FS
{
    /// <summary>
    /// Root element of the file system
    /// </summary>
    public class Root
    {
        private String _label;

        /// <summary>
        /// Markup
        /// </summary>
        public String Label 
        {
            get { return _label; }
            set { _label = value; }
        }
    }
}
