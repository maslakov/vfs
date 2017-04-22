using System;
using Core.Sessions;
using Core.FsExceptions;

namespace Core.FS.Commands
{
    /// <summary>
    /// Move file or directory command
    /// </summary>
    public class MoveCommand : AbstractCommand
    {
        readonly string _from;
        readonly string _to;

        /// <summary>
        /// Command: move file or directory
        /// </summary>
        /// <param name="vfs">Target file system</param>
        /// <param name="fromItemName">From where</param>
        /// <param name="toItemName">To where</param>
        /// <param name="userToken">User descriptor</param>
        public MoveCommand(IVirtualFileSystem vfs, String fromItemName, String toItemName, SID userToken)
            : base(userToken, vfs)
        {
            _from = fromItemName;
            _to = toItemName;

        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <returns></returns>
        public override CommandResponse Execute()
        {
            CommandResponse response = new CommandResponse();
            try
            {
                Storage.MoveItem(CurrentDirectory,_from, _to, UserToken);
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
