using System;
using Core.Sessions;
using Core.FsExceptions;

namespace Core.FS.Commands
{
    /// <summary>
    /// Copy directory or file 
    /// </summary>
    public class CopyCommand : AbstractCommand
    {
        readonly string _from;
        readonly string _to;

        public CopyCommand(IVirtualFileSystem vfs, String fromItemName, String toItemName, SID userToken)
            : base(userToken,vfs)
        {
            _from = fromItemName;
            _to = toItemName;
        }

        public override CommandResponse Execute()
        {
            CommandResponse response = new CommandResponse();
            try
            {
                Storage.CopyItem(CurrentDirectory, _from, _to, UserToken);
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
