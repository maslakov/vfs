using System.ServiceModel;

namespace Core.Interfaces.Service
{
    /// <summary>
    /// Interface of virtual file system
    /// </summary>
    [ServiceContract(SessionMode=SessionMode.Required, 
        CallbackContract = typeof (IClientContractCallback))]
    public interface IVfsService
    {
        /// <summary>
        /// Open new session
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>Session descriptor</returns>
        [OperationContract(IsInitiating=true)]
        SessionInfo CreateSession(string userName);

        /// <summary>
        /// Process user command
        /// </summary>
        /// <param name="command">Text of the command with arguments</param>
        /// <returns>Processing result</returns>
        [OperationContract(IsInitiating = false)]
        CommandResult ProcessCommand(string command);

        /// <summary>
        /// Close the session
        /// </summary>
        [OperationContract(IsTerminating=true)]
        void Quit();

        [OperationContract]
        string Test();
    }

    /// <summary>
    /// Callback interface for duplex channel
    /// </summary>
    public interface IClientContractCallback
    {
        /// <summary>
        /// Send text notification to users
        /// </summary>
        /// <param name="Message">Text of the notification</param>
        /// <param name="UserName">Initializer</param>
        [OperationContract(IsOneWay = true)]
        void OnNotify(string Message, string UserName);
    }

}
