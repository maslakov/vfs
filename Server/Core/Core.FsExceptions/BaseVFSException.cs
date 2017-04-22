using System;

namespace Core.FsExceptions
{
    /// <summary>
    /// Base file system exception
    /// </summary>
    public class BaseVFSException : Exception
    {
        public BaseVFSException()
        { 
        
        }

        public BaseVFSException(string message) : base(message) 
        { 
        }
    }
}
