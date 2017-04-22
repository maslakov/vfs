using System;
using Core.Sessions;
using Core.FsExceptions;

namespace Core.FS.Commands
{
    /// <summary>
    /// Delete directory tree
    /// </summary>
    public class DelTreeCommand : AbstractCommand
    {
        private readonly String _rootDir;

        /// <summary>
        /// Command for directory deletion
        /// </summary>
        /// <param name="vfs">Target file system</param>
        /// <param name="directoryName">Directory name to be deleted</param>
        /// <param name="userToken">User descriptor</param>
        public DelTreeCommand(IVirtualFileSystem vfs, String directoryName, SID userToken)
            : base(userToken,vfs)
        {
            _rootDir = directoryName;
        }

        public override CommandResponse Execute()
        {
            CommandResponse response = new CommandResponse();
            try
            {
                Storage.DeleteItemTree(CurrentDirectory, _rootDir, UserToken);
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
