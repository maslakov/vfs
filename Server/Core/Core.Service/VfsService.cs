using System;
using System.Linq;
using Core.Interfaces.Service;
using Core.FS;
using Core.Sessions;
using System.ServiceModel;
using Core.FsExceptions;
using Core.FS.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Service
{
    /// <summary>
    /// Implementation of the WCF service with virtual file system. 
    /// With multithreading support. 
    /// With sessions support.
    /// </summary>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession, 
        ConcurrencyMode=ConcurrencyMode.Multiple
    )]
    public class VfsService : IVfsService
    {
        // singleton references to session manager and virtual file system object
        private static readonly IVirtualFileSystem _fs;
        private static readonly ISessionManager _sessions;

        #region event handlers      
 
        /// <summary>
        /// Event to send a message to the client using approach #2
        /// </summary>
        public static event VfsChangeEventHandler FSChangeEvent;

        /// <summary>
        /// Event to send a message to the client using approach #1
        /// </summary>
        public static event VfsSimpleChangeEventHandler SimpleFSChangeEvent;

        /// <summary>
        /// Handler type for client notification
        /// </summary>
        public delegate void VfsChangeEventHandler(object sender, FSChangedEventArgs e);

        /// <summary>
        /// Handler type for client notification
        /// </summary>
        public delegate void VfsSimpleChangeEventHandler(object sender, FsEventArgs e);

        /// <summary>
        /// Client callback
        /// </summary>
        IClientContractCallback callback = null;

        /// <summary>
        /// Handler for client notification
        /// </summary>
        VfsChangeEventHandler vfsChangeHandler = null;
        
        /// <summary>
        /// Handler for client notification
        /// </summary>
        VfsSimpleChangeEventHandler simpleVfsHandler = null;


        /// <summary>
        /// Something was copied
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Who and what</param>
        private static void OnCopyHandler(object sender, StorageChangedEventArgs e)
        {
            if (FSChangeEvent!=null)
                FSChangeEvent(sender, new FSChangedEventArgs(e, FsChangeType.COPY));
        }

        /// <summary>
        /// Something was deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Who and what</param>
        private static void OnDeleteHandler(object sender, StorageChangedEventArgs e)
        {
            if (FSChangeEvent != null)
                FSChangeEvent(sender, new FSChangedEventArgs(e, FsChangeType.DELETE));
        }

        /// <summary>
        /// Something was created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Who and what</param>
        private static void OnCreateHandler(object sender, StorageChangedEventArgs e)
        {
            if (FSChangeEvent != null)
                FSChangeEvent(sender, new FSChangedEventArgs(e, FsChangeType.CREATE));
        }

        /// <summary>
        /// Something was moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Who and what</param>
        private static void OnMoveHandler(object sender, StorageChangedEventArgs e)
        {
            if (FSChangeEvent != null)
                FSChangeEvent(sender, new FSChangedEventArgs(e, FsChangeType.MOVE));
        } 

        /// <summary>
        /// Send a message to the client based on the event caught from the file system
        /// </summary>
        public void FSChangeHandler(object sender, FSChangedEventArgs e)
        {
            callback.OnNotify(String.Format("{0} {1} by User {2}",
                e.Item.FullName, GetOperationActionVerb(e), e.UserToken.Label),e.UserToken.Label);
        }

        private static string GetOperationActionVerb(FSChangedEventArgs e)
        {
            return e.Type == FsChangeType.COPY ?  "copied" :
                e.Type == FsChangeType.DELETE ? "deleted" :
                    e.Type == FsChangeType.MOVE ? "moved" :
                        e.Type == FsChangeType.CREATE ? "created" : "";
        }

        /// <summary>
        /// Send a message to the client about command execution
        /// </summary>
        public void SimpleFSChangeHandler(object sender, FsEventArgs e)
        {
            callback.OnNotify(String.Format("User {0} performs command: {1}",
                e.UserToken.Label,
                e.CommandName
            ), e.UserToken.Label);

        }
        #endregion

        public Guid instanceId;

        /// <summary>
        /// Static initialization of WCF service class
        /// </summary>
        static VfsService()
        {
            //Singleton - the only session manager for all service instances
            _sessions = SessionManager.Instance;

            //Singleton - the only file system for all service instances
            _fs = InMemoryFileSystem.Instance;

            _fs.CurrentSessionsManager = _sessions;

            // create subscriptions to file system events for sending notifications to all service instances
            _fs.CurrentStorage.OnItemCopy += OnCopyHandler;
            _fs.CurrentStorage.OnItemDeleted += OnDeleteHandler;
            _fs.CurrentStorage.OnItemCreated += OnCreateHandler;
            _fs.CurrentStorage.OnItemMoved += OnMoveHandler;        
        }
        
        /// <summary>
        /// Creates an instance of WCF service. 
        /// One instance per session.
        /// </summary>
        public VfsService()
        {
            instanceId = Guid.NewGuid();

            Console.WriteLine("constructor call " + Thread.CurrentThread.ManagedThreadId.ToString());
        }


        /// <summary>
        /// Open new session
        /// </summary>
        /// <param name="userName">the name of user</param>
        /// <returns>Returns session descriptor with session identifier and the number of users, connected to the server</returns>
        public SessionInfo CreateSession(string userName)
        {
            
            Console.WriteLine(" Instance:" + instanceId.ToString() + " Thread:" + Thread.CurrentThread.ManagedThreadId.ToString() + "\n\n");

            UserSession sess = _sessions.GetSession(OperationContext.Current.SessionId);
            if (sess !=null)
                throw new FaultException(String.Format("There is already a session opened for user: {0}",sess.UserName));

            try
            {
                // create new session
                UserSession newUser = _sessions.CreateSession(
                    userName,
                    new DirectoryItem("\\", "C:"),
                    OperationContext.Current.SessionId);

                Console.WriteLine("{0}, {1}", userName, OperationContext.Current.SessionId);

                // subscribe user to file system changes
                
                callback = OperationContext.Current.GetCallbackChannel<IClientContractCallback>();
                //approach #1 - using events from the service itself
                simpleVfsHandler = new VfsSimpleChangeEventHandler(SimpleFSChangeHandler);
                SimpleFSChangeEvent += simpleVfsHandler;
                //approach #2 - using events from file system operations. inform user about the commands executed
                vfsChangeHandler = new VfsChangeEventHandler(FSChangeHandler);
                FSChangeEvent += vfsChangeHandler;

                //return ID, current directory, number of connected users
                return new SessionInfo 
                {
                    Sid = newUser.UserToken.Token, 
                    UserConnected = _sessions.ConnectedUsers().Count(),
                    CurrentDir = newUser.CurrentDirectory.FullName
                };

            }
            catch (SessionException e)
            {
                throw new FaultException(e.Message);
            }


        }

 
        /// <summary>
        /// Process user command
        /// </summary>
        /// <param name="command">incoming command with all arguments as single string</param>
        /// <returns></returns>
        public CommandResult ProcessCommand(string command)
        {
            
            Console.WriteLine(" Instance:" + instanceId.ToString() + " Thread:" + Thread.CurrentThread.ManagedThreadId.ToString() + "\n\n");

            UserSession sess = _sessions.GetSession(OperationContext.Current.SessionId);
            if (sess != null)
            {
                try
                {
                    //process the command in the file system
                    CommandResponse rs = _fs.ProcessCommand(command, sess.UserToken);

                    if (rs.LastError != null)
                    {
                        // there was an error
                        Console.WriteLine("{0}", rs.LastError.Message);
                        throw new FaultException(rs.LastError.Message);
                    }

                    if (rs.Successful && rs.Changed)
                    {
                        // Raise an event by service - inform all subscribers
                        if (SimpleFSChangeEvent != null)
                            SimpleFSChangeEvent(this, new FsEventArgs { UserToken = sess.UserToken, CommandName = command });
                    }
                    return new CommandResult 
                    { 
                        TextResult = rs.TextsResult, 
                        CurrentDir = sess.CurrentDirectory.FullName 
                    };
                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
            }
            else
                throw new FaultException("Session is not opened! Use CONNECT command");
        }

        /// <summary>
        /// Close session
        /// </summary>
        public void Quit()
        {
            Console.WriteLine("QUIT {0}",  OperationContext.Current.SessionId);
            _sessions.CloseSession(OperationContext.Current.SessionId);
            //unsubscribe this instance from all events
            FSChangeEvent -= vfsChangeHandler;
            SimpleFSChangeEvent -= simpleVfsHandler;
        }

        /// <summary>
        /// Test
        /// </summary>
        /// <returns>List of all users connected</returns>
        public string Test()
        {
            string result = String.Join("\n", _sessions.ConnectedUsers().ToArray());
            UserSession sess = _sessions.GetSession(OperationContext.Current.SessionId);

            IClientContractCallback callbackTemp = OperationContext.Current.GetCallbackChannel<IClientContractCallback>();

            Task.Run(() => { callbackTemp.OnNotify("Message: " + result, sess.UserName); });

            return result;
        }

      
    }
}
