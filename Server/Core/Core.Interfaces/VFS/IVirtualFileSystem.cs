using System;
using Core.Sessions;
using Core.FS.Commands;

namespace Core.FS
{
    /// <summary>
    /// Virtual file system
    /// </summary>
    public interface IVirtualFileSystem
    {
        /// <summary>
        /// Session manager
        /// </summary>
        ISessionManager CurrentSessionsManager { get; set; }
        
        /// <summary>
        /// Storage
        /// </summary>
        IStorage CurrentStorage { get; set; }

        /// <summary>
        /// Command processor
        /// </summary>
        /// <param name="commandString">Command string</param>
        /// <param name="userToken">User</param>
        /// <returns>Command processing result</returns>
        CommandResponse ProcessCommand(String commandString, SID userToken);

    }
}
