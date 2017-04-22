using System.Runtime.Serialization;

namespace Core.Interfaces.Service
{
    /// <summary>
    /// Data to be returned to the client by the end of command execution
    /// </summary>
    [DataContract]
    public class CommandResult
    {
        /// <summary>
        /// Text output
        /// </summary>
        [DataMember]
        public string TextResult { get; set; }
        
        /// <summary>
        /// Current directory
        /// </summary>
        [DataMember]
        public string CurrentDir { get; set; }
    }
}
