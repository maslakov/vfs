using System;

namespace Core.FS.Commands
{
    /// <summary>
    /// Results of the command processing by the file system
    /// </summary>
    public class CommandResponse
    {
        /// <summary>
        /// If everything went fine
        /// </summary>
        public bool Successful { get; set; }

        /// <summary>
        /// Textual output
        /// </summary>
        public String TextsResult { get; set; }

        /// <summary>
        /// Exception during processing
        /// </summary>
        public Exception LastError { get; set; }

        /// <summary>
        /// Whether something was changed in the file system or not
        /// </summary>
        public bool Changed { get; set; }
    }
}
