using System;
using Core.Sessions;
using Core.FsExceptions;

namespace Core.FS.Commands
{
    /// <summary>
    /// Lock file
    /// </summary>
    public class LockCommand : AbstractCommand
    {
        private readonly string _itemToLock;

        public LockCommand(IVirtualFileSystem vfs, String itemName, SID userToken)
            : base(userToken, vfs)
        {
            _itemToLock = itemName;
        }

        public override CommandResponse Execute()
        {
            CommandResponse response = new CommandResponse();
            try
            {
                if (Session != null)
                {
                    Storage.LockItem(CurrentDirectory, _itemToLock, UserToken);
                    response.Changed = true;
                    response.Successful = true;
                }
                else
                {
                    response.Successful = false;
                    response.LastError = new SessionException("Session lost!");
                }

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
