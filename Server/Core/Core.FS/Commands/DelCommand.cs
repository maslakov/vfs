using System;
using Core.Sessions;
using Core.FsExceptions;

namespace Core.FS.Commands
{
    /// <summary>
    /// delete file
    /// </summary>
    public class DelCommand : AbstractCommand
    {
        private readonly string _delFile;

        /// <summary>
        /// Create a command for file deletion
        /// </summary>
        /// <param name="vfs">Target file system</param>
        /// <param name="fileName">Path to the file to be deleted</param>
        /// <param name="userToken">User descriptor</param>
        public DelCommand(IVirtualFileSystem vfs, String fileName, SID userToken)
            : base(userToken, vfs)
        {
            _delFile = fileName;
        }

        public override CommandResponse Execute()
        {
            CommandResponse response = new CommandResponse();
            try
            {
                Storage.DeleteFile(CurrentDirectory, _delFile, UserToken);
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
