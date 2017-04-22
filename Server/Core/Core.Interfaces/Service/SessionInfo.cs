using System;
using System.Runtime.Serialization;

namespace Core.Interfaces.Service
{
    /// <summary>
    /// Session descriptor. Returned when session is opened
    /// </summary>
    [DataContract]
    public class SessionInfo
    {
        /// <summary>
        /// Session ID
        /// </summary>
        [DataMember]
        public String Sid { get; set; }

        /// <summary>
        /// Number of users connected
        /// </summary>
        [DataMember]
        public int UserConnected { get; set; }

        /// <summary>
        /// Current directory
        /// </summary>
        [DataMember]
        public string CurrentDir { get; set; }
    }
}
