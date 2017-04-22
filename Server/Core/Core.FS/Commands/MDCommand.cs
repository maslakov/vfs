using System;
using Core.FsExceptions;
using Core.Sessions;

namespace Core.FS.Commands
{
    /// <summary>
    /// Create directory command
    /// </summary>
    public class MDCommand : AbstractCommand
    {
        /// <summary>
        /// Name of new directory
        /// </summary>
        private readonly String _newDirName;

        /// <summary>
        /// Create an instance of the command
        /// </summary>
        /// <param name="vfs">File system to be used</param>
        /// <param name="directoryName">New directory name</param>
        /// <param name="userToken">User descriptor</param>
        public MDCommand(IVirtualFileSystem vfs, String directoryName, SID userToken)
            : base(userToken,vfs)
        {
            _newDirName = directoryName;
        }

        public override CommandResponse Execute()
        {
            CommandResponse response = new CommandResponse();
            try
            {
                Storage.CreateDirectory(CurrentDirectory,_newDirName,UserToken);
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
