using Core.FsExceptions;
using Core.Sessions;

namespace Core.FS.Commands
{
    /// <summary>
    /// Print file system tree command
    /// </summary>
    public class PrintCommand : AbstractCommand
    {
        /// <summary>
        /// Print textual representation of file system tree
        /// </summary>
        public PrintCommand(IVirtualFileSystem vfs, SID userToken)
            : base(userToken,vfs)
        {

        }

        public override CommandResponse Execute()
        {
            CommandResponse response = new CommandResponse();
            try
            {
                response.TextsResult = Storage.GetPrintableView(UserToken);
                response.Successful = true;
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
