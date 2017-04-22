using System;
using System.ServiceModel;
using Core.Interfaces.Service;

namespace ConsoleClient
{
    public class WorkSession
    {
        /// <summary>
        /// Session ID
        /// </summary>
        private string _sid = String.Empty;

        /// <summary>
        /// Remote service
        /// </summary>
        private IVfsService _vfsService = null;

        /// <summary>
        /// Communication channel
        /// </summary>
        private DuplexChannelFactory<IVfsService> _channelFactory = null;

        /// <summary>
        /// Current login
        /// </summary>
        private string _currentLogin = String.Empty;

        private string Login
        {
            get { return _currentLogin; }
        }

        private string CurrentDidectory { get; set; }

        /// <summary>
        /// Session is closed flag
        /// </summary>
        private bool _closed = false;

        public bool Closed
        {
            get { return _closed; }
        }

        /// <summary>
        /// Connection is closed flag
        /// </summary>
        private bool _ended = false;

        public bool Ended
        {
            get { return _ended; }
        }

        private VfsClientEventHandler _serverMsgHandler = null;

        /// <summary>
        /// Show server messages
        /// </summary>
        private void MessageRecieved(object sender, VfsClientEventArgs e)
        {
            // Show message if we are not a source of it
            if (e.User != this.Login)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(">>");
            }
        }

        /// <summary>
        /// Promt the command from user
        /// </summary>
        /// <returns></returns>
        private string Promt()
        {
            string cmd;
            Console.Write("{0}>>", CurrentDidectory);
            cmd = Console.ReadLine();
            return cmd;
        }

        /// <summary>
        /// Closing of the channel
        /// </summary>
        private void Close()
        {
            try
            {
                if (_channelFactory != null)
                {
                    if (_channelFactory.State == CommunicationState.Opened)
                        _channelFactory.Close();
                    else if (_channelFactory.State == CommunicationState.Faulted)
                        _channelFactory.Abort();
                }
                if (_vfsService != null)
                {
                    ((IClientChannel) _vfsService).Close();
                    ((IDuplexContextChannel) _vfsService).Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection error: {0}", e.Message);
            }
            finally
            {
                _ended = true;
            }
        }

        /// <summary>
        /// Main worker cycle
        /// </summary>
        public void Start()
        {
            // setup server messages handler
            _serverMsgHandler = new VfsClientEventHandler(MessageRecieved);
            ClientCallback.OnServerMessage += _serverMsgHandler;
            
            string cmd;

            do
            {
                cmd = Promt();
                if (cmd == null)
                {
                    //Ctrl+C
                    Quit();
                    break;
                }
                
                string[] args = cmd.Split(new Char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length > 0)
                {
                    if (args[0].ToUpper() == "CONNECT")
                    {
                        ConnectToServer(args);
                    }
                    else
                    {
                        // Exit
                        if (args[0].ToUpper() == "QUIT")
                        {
                            cmd = args[0];
                            Quit();
                        }
                        else if (args[0].ToUpper() == "TEST")
                        {
                            Test();
                        }
                        else
                        {
                            #region processing of commands

                            try
                            {
                                if (!String.IsNullOrEmpty(_sid))
                                {
                                    // check the channel
                                    if (_vfsService != null
                                        && ((IClientChannel) _vfsService).State == CommunicationState.Opened)
                                    {
                                        // call the service
                                        CommandResult response = _vfsService.ProcessCommand(cmd);

                                        // print response
                                        if (!String.IsNullOrEmpty(response.TextResult))
                                            Console.WriteLine(response.TextResult);
                                            
                                        // set current folder from response
                                        CurrentDidectory = response.CurrentDir;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Connection lost...... Please restart the client");
                                        break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("There is no opened session! Please use CONNECT command");
                                }
                            }
                            catch (FaultException fe)
                            {
                                Console.WriteLine(fe.Message);
                            }
                            catch (CommunicationException ce)
                            {
                                Console.WriteLine(ce.Message);
                                ((IClientChannel) _vfsService).Abort();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                ((IClientChannel) _vfsService).Abort();
                            }

                            #endregion
                        }
                    }
                }
                
            } while (cmd.ToUpper() != "QUIT" && _ended != true);

            ClientCallback.OnServerMessage -= _serverMsgHandler;

            Close();
        }

        private void ConnectToServer(string[] args)
        {
            // if there is no active session
            if (String.IsNullOrEmpty(_sid)
                ||
                (_vfsService != null &&
                 (((IClientChannel) _vfsService).State == CommunicationState.Faulted ||
                     ((IClientChannel) _vfsService).State == CommunicationState.Closed)))
            {
                if (args.Length == 3)
                {
                    string _vfsAddress = args[1];
                    NetTcpBinding binding = new NetTcpBinding
                    {
                        OpenTimeout = TimeSpan.FromSeconds(90),
                        ReceiveTimeout = TimeSpan.FromSeconds(480),
                        SendTimeout = TimeSpan.FromSeconds(90)
                    };

                    if (!_vfsAddress.StartsWith("net.tcp://"))
                        _vfsAddress = "net.tcp://" + _vfsAddress;

                    EndpointAddress address = new EndpointAddress(_vfsAddress);

                    //setup a callback for duplex channel
                    InstanceContext ctx = new InstanceContext(new ClientCallback());

                    //open the channel
                    _channelFactory = new DuplexChannelFactory<IVfsService>(ctx, binding, address);
                    try
                    {
                        _vfsService = _channelFactory.CreateChannel();
                        
                        _currentLogin = args[2].Trim();
                        
                        SessionInfo response = _vfsService.CreateSession(_currentLogin);
                        _sid = response.Sid;
                        CurrentDidectory = response.CurrentDir;
                        Console.WriteLine("Connected {0} users.", response.UserConnected);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Wrong command format!");
                }
            }
            else
            {
                Console.WriteLine("There is opened session already!");
            }
        }

        /// <summary>
        /// Test callback
        /// </summary>
        public void Test()
        {
            try
            {
                if (_vfsService != null
                    && ((IClientChannel) _vfsService).State == CommunicationState.Opened)
                {
                    _vfsService.Test();
                }
                else
                {
                    Console.WriteLine("Connection lost...... TRY RECONNECT");
                }
            }
            catch (FaultException fe)
            {
                Console.WriteLine(fe.Message);
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine(ce.Message);
                ((IClientChannel) _vfsService).Abort();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ((IClientChannel) _vfsService).Abort();
            }
        }

        /// <summary>
        /// Closing of the session on the server
        /// </summary>
        public void Quit()
        {
            try
            {
                // if the channel is alive 
                if (_vfsService != null
                    && ((IClientChannel) _vfsService).State == CommunicationState.Opened)
                {
                    // close the session
                    _vfsService.Quit();
                    _closed = true;
                }
            }
            catch (FaultException fe)
            {
                Console.WriteLine(fe.Message);
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine(ce.Message);
                ((IClientChannel) _vfsService).Abort();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ((IClientChannel) _vfsService).Abort();
            }
            finally
            {
                _ended = true;
            }
        }
    }
}