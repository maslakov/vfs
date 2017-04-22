using Core.FS.Commands;
using Core.Sessions;
using Core.FS.Storage;

namespace Core.FS
{
    /// <summary>
    /// Implementation: 
    /// virtual file system with a storage based on the tree, which is implemented based on linked lists
    /// </summary>
    public class InMemoryFileSystem :IVirtualFileSystem
    {
        /// <summary>
        /// Session manager
        /// </summary>
        public ISessionManager CurrentSessionsManager { get; set; }

        /// <summary>
        ///Storage
        /// </summary>
        public IStorage CurrentStorage { get; set; }

        /// <summary>
        /// Process the incoming command
        /// </summary>
        /// <param name="commandString">Text of the command with arguments</param>
        /// <param name="userToken">Descriptor of the user who performs the command</param>
        /// <returns>Command processing result</returns>
        public CommandResponse ProcessCommand(string commandString, SID userToken)
        {
            AbstractCommand cmd = CommandFactroy.CreateCommand(this, commandString, userToken);
            CommandResponse response = cmd.Execute();
            return response;
        }

        /// <summary>
        /// Creates an instance of file system
        /// </summary>
        private InMemoryFileSystem()
        {
            CurrentStorage = new VirtualStorage();
        }

        /// <summary>
        /// Singleton implementation
        /// </summary>
        public static InMemoryFileSystem Instance { get { return InstanceHolder._instance; } }

        /// <summary>
        /// Class handler. Instantiated by first call
        /// </summary>
        private class InstanceHolder
        {
            static InstanceHolder() { }

            internal static readonly InMemoryFileSystem _instance = new InMemoryFileSystem();
        }


    }
}
