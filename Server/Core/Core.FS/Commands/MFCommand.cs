using System;
using Core.Sessions;
using Core.FsExceptions;

namespace Core.FS.Commands
{
    /// <summary>
    /// Create file command
    /// </summary>
    public class MFCommand : AbstractCommand
    {
        /// <summary>
        /// New file name
        /// </summary>
        private readonly String _newFileName;

        /// <summary>
        /// Create new file command
        /// </summary>
        /// <param name="vfs">Target file system</param>
        /// <param name="fileName">File name</param>
        /// <param name="userToken">User descriptor</param>
        public MFCommand(IVirtualFileSystem vfs, String fileName, SID userToken)
            : base(userToken, vfs)
        {
            _newFileName = fileName;
        }

        public override CommandResponse Execute()
        {
            CommandResponse response = new CommandResponse();
            try
            {
                Storage.CreateFile(CurrentDirectory,_newFileName,UserToken);
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
