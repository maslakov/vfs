using System;
using Core.Sessions;
using Core.FsExceptions;

namespace Core.FS.Commands
{
    /// <summary>
    /// Delete directory command
    /// </summary>
    public class RDCommand : AbstractCommand
    {
        private readonly string _delDir;

        /// <summary>
        /// delete directory command
        /// </summary>
        /// <param name="vfs">Target file system</param>
        /// <param name="directoryName">Directory to be deleted</param>
        /// <param name="userToken">User descriptor</param>
        public RDCommand(IVirtualFileSystem vfs, String directoryName, SID userToken)
            : base(userToken, vfs)
        {
            _delDir = directoryName;
        }

        public override CommandResponse Execute()
        {
            CommandResponse response = new CommandResponse();
            try
            {
                Storage.DeleteDirectory(CurrentDirectory, _delDir, UserToken);
                response.Successful = true;
                response.Changed = true;
            }
            catch (BaseVFSException e)
            {
                response.Successful = false;
                response.LastError = e;
            }

            return response;
        }
    }
}
