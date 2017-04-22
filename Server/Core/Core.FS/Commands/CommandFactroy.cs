using System;
using Core.FsExceptions;
using Core.Sessions;


namespace Core.FS.Commands
{
    public static class CommandFactroy
    {
        /// <summary>
        /// Factory which creates commands based on their string representation with all arguments
        /// </summary>
        /// <param name="vfs">Target file system</param>
        /// <param name="commandString">Command string with parameters</param>
        /// <param name="userToken">Current user descriptor</param>
        /// <returns></returns>
        public static AbstractCommand CreateCommand(IVirtualFileSystem vfs, String commandString, SID userToken)
        {
            if (String.IsNullOrEmpty(commandString))
                throw new ParamsNotAllowedException(AbstractCommand.BadCommandMessage);

            string[] args = commandString.Split(' ');
            
            ValidateArguments(args);

            string cmd = args[0].ToUpper();
            AbstractCommand resultCommand;

            switch(cmd )
            {
                case "MD": resultCommand = new MDCommand(vfs, args[1],  userToken);
                    break;
                case "CD": resultCommand = new CDCommand(vfs, args[1], userToken);
                    break;
                case "RD": resultCommand = new RDCommand(vfs, args[1], userToken);
                    break;
                case "DELTREE": resultCommand = new DelTreeCommand(vfs, args[1], userToken);
                    break;
                case "MF": resultCommand = new MFCommand(vfs, args[1], userToken);
                    break;
                case "DEL": resultCommand = new DelCommand(vfs, args[1], userToken);
                    break;
                case "LOCK": resultCommand = new LockCommand(vfs, args[1], userToken);
                    break;
                case "UNLOCK": resultCommand = new UnlockCommand(vfs, args[1], userToken);
                    break;
                case "COPY": resultCommand = new CopyCommand(vfs, args[1], args[2], userToken);
                    break;
                case "MOVE": resultCommand = new MoveCommand(vfs, args[1], args[2], userToken);
                    break;
                case "PRINT": resultCommand = new PrintCommand(vfs, userToken);
                    break;
                default:
                    throw new ParamsNotAllowedException(AbstractCommand.BadCommandMessage);
            }
            
            return resultCommand;

        }

        private static void ValidateArguments(string[] args)
        {
            string cmd = args[0].ToUpper();
            if (cmd != "PRINT")
            {
                if (args.Length == 1)
                    throw new ParamsNotAllowedException(AbstractCommand.BadCommandMessage);
            }
            else if (cmd == "COPY" || cmd == "MOVE")
            {
                if (args.Length < 2)
                    throw new ParamsNotAllowedException(AbstractCommand.BadCommandMessage);
            }
        }
    }
}
