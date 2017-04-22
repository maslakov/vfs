using System;
using Core.Sessions;

namespace Core.FS.Commands
{
    public abstract class AbstractCommand
    {
        public const string BadCommandMessage = "Wrong command format";

        /// <summary>
        /// Storage to perform command on
        /// </summary>
        protected readonly IStorage Storage;

        /// <summary>
        /// Actual user session
        /// </summary>
        protected readonly UserSession Session;

        /// <summary>
        /// Current user directory
        /// </summary>
        protected readonly DirectoryItem CurrentDirectory;

        /// <summary>
        /// User descriptor
        /// </summary>
        protected SID UserToken;

        private AbstractCommand(IVirtualFileSystem vfs)
        {
            if (vfs == null)
            {
                throw new ArgumentNullException("vfs");
            }

            Session = null;
            
            Storage = vfs.CurrentStorage;
        }

        protected AbstractCommand(SID userToken, IVirtualFileSystem vfs)
            : this(vfs)
        {
            Session = vfs.CurrentSessionsManager.GetSession(userToken.Token);

            if (Session != null)
                CurrentDirectory = Session.CurrentDirectory;
            UserToken = userToken;
        }

        public abstract CommandResponse Execute();
    }
}
