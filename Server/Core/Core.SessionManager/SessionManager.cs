using System;
using System.Collections.Generic;
using System.Linq;
using Core.FS;
using Core.FsExceptions;

namespace Core.Sessions
{
    /// <summary>
    /// Session manager for virtual file system
    /// </summary>
    public class SessionManager : ISessionManager
    {
        /// <summary>
        /// List of active sessions
        /// </summary>
        private readonly Dictionary<string, UserSession> _usersList;

        private SessionManager()
        {
            _usersList = new Dictionary<string, UserSession>();
        }


        /// <summary>
        /// Singleton implementation
        /// </summary>
        public static SessionManager Instance { get { return InstanceHolder._instance; } }

        /// <summary>
        /// class-handler for the instance with private constructor
        /// will be initialized by the first invocation
        /// </summary>
        private class InstanceHolder
        {
            static InstanceHolder() { }

            internal static readonly SessionManager _instance = new SessionManager();
        }

        /// <summary>
        /// Check if the user is already logged in
        /// </summary>
        /// <param name="userName">user name</param>
        /// <returns>true/false</returns>
        private bool ExistSession(string userName)
        {
            bool userExist = false;

            lock (_usersList)
                userExist = _usersList.Any(kv => kv.Value.UserName == userName);
            
            return userExist;
        
        }

        /// <summary>
        /// Open new session
        /// </summary>
        /// <param name="userName">name of new user</param>
        /// <param name="currentDir">user starting folder</param>
        /// <param name="sessionID">Session ID</param>
        /// <returns>Session descriptor object</returns>
        public UserSession CreateSession(String userName, DirectoryItem currentDir, String sessionID)
        {
            UserSession newUser = new UserSession
            {
                UserName = userName,
                UserToken = new SID { Token = sessionID, Label = userName },
                CurrentDirectory = currentDir,
                ServiceSessionID = sessionID
            };
            
            lock (_usersList)
            {
                if(ExistSession(userName))
                    throw new SessionException("There is already a user with provided name connected!");
                _usersList.Add(newUser.ServiceSessionID, newUser);
            }

            return newUser;
        }

        /// <summary>
        /// Find a session by user identifier
        /// </summary>
        /// <param name="sid">user session unique identifier</param>
        /// <returns>Session descriptor object if any</returns>
        public UserSession GetSession(string sid)
        {
            UserSession user = null;
            
            lock (_usersList)
                _usersList.TryGetValue(sid, out user);
            
            return user;
        }

        /// <summary>
        /// Close the session for user
        /// </summary>
        /// <param name="sid">user session unique identifier</param>
        public void CloseSession(string sid)
        {
            lock (_usersList)
                _usersList.Remove(sid);
        }

        /// <summary>
        /// List of all active sessions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ConnectedUsers()
        {
            return _usersList.Select(u=>u.Value.UserName).AsEnumerable();
        }
    }
}
