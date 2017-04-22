using Core.FS;

namespace Core.Sessions
{
 
    /// <summary>
    /// User session descriptor
    /// </summary>
    public class UserSession
    {
        /// <summary>
        /// Meaningful name: login
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User unique descriptor. In our case it is user login
        /// </summary>
        public SID UserToken { get; set; }

        /// <summary>
        /// Current user catalog
        /// </summary>
        public DirectoryItem CurrentDirectory { get; set; }

        /// <summary>
        /// Session identifier
        /// </summary>
        public string ServiceSessionID { get; set; }
    }
}
