using System;
using Core.FS;
using System.Collections.Generic;
namespace Core.Sessions
{
    /// <summary>
    /// sessions manager interface
    /// </summary>
    public interface ISessionManager
    {
        /// <summary>
        /// Close session by id
        /// </summary>
        /// <param name="sid">Session ID</param>
        void CloseSession(String sid);
        
        /// <summary>
        /// Open new sesion
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="currentDir">Current folder</param>
        /// <param name="sessionId">session ID</param>
        /// <returns>User session descriptor</returns>
        UserSession CreateSession(string userName, DirectoryItem currentDir, String sessionId);
        
        /// <summary>
        /// Find opened sesion by ID
        /// </summary>
        /// <param name="sid">session ID to be searched</param>
        /// <returns>User session descriptor</returns>
        UserSession GetSession(String sid);
        
        /// <summary>
        /// Get the list of the names of all users connected
        /// </summary>
        IEnumerable<string> ConnectedUsers();
    }
}
