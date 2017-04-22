using System;
using Core.Interfaces.Service;

namespace ConsoleClient
{
    public class VfsClientEventArgs : EventArgs
    {
        public string Message { get; set; }
        public string User { get; set; }
    }

    public delegate void VfsClientEventHandler(object sender, VfsClientEventArgs e);

    /// <summary>
    /// Client callback for duplex channel
    /// </summary>
    public class ClientCallback : IClientContractCallback
    {
        public static event VfsClientEventHandler OnServerMessage;

        public void OnNotify(string message, string user)
        {

            if (OnServerMessage!=null)
            {
                OnServerMessage(this, new VfsClientEventArgs { Message = message, User = user });    
            }
        }
    }
}
