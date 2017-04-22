using System;
using Core.FsExceptions;
using Core.Sessions;

namespace Core.FS.Commands
{
    /// <summary>
    /// Make a directory a current one
    /// </summary>
    public class CDCommand : AbstractCommand
    {
        private string _moveToDir;

        /// <summary>
        /// Create an instance of the command
        /// </summary>
        /// <param name="directoryName">Directory name to move into</param>
         /// <param name="userToken">User descriptor</param>
        public CDCommand(IVirtualFileSystem vfs, String directoryName, SID userToken)
            : base(userToken,vfs)
        {
            _moveToDir = directoryName;
        }

        /// <summary>
        /// Execute command: make a directory the current one
        /// </summary>
        /// <returns></returns>
        public override CommandResponse Execute()
        {
            CommandResponse response = new CommandResponse();
            try
            {
                if (Session != null)
                {
                    FsItem newDir = Storage.FindItem(CurrentDirectory, _moveToDir, UserToken);
                    if (newDir != null)
                    {
                        response.Successful = true;
                        Session.CurrentDirectory = (DirectoryItem)newDir;
                    }
                    else
                    {
                        response.Successful = false;
                        response.LastError = new NotFoundException("Directory was not found!");
                    }
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
