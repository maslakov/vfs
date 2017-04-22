using System;
using Core.Sessions;
using Core.FsExceptions;

namespace Core.FS.Commands
{
    /// <summary>
    /// Unlock file
    /// </summary>
    public class UnlockCommand : AbstractCommand
    {
        private readonly string _itemToUnLock;

        public UnlockCommand(IVirtualFileSystem vfs, String itemName, SID userToken)
            : base(userToken,vfs)
        {
            _itemToUnLock = itemName;
        }

        public override CommandResponse Execute()
        {
            CommandResponse response = new CommandResponse();
            try
            {
                if (Session != null)
                {
                    Storage.UnLockItem(CurrentDirectory, _itemToUnLock, UserToken);
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
