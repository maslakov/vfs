using System;

using System.ServiceModel;
using Core.Service;
using System.ServiceProcess;

namespace Host.WinService
{
    public class VfsWinService : ServiceBase , IManualStartService
    {
        #region private fields

        /// <summary>
        /// Windows service parameters
        /// </summary>
        private readonly ServiceParameters _serviceParameters = new ServiceParameters();

        private int _nbRestartRequested = 0;
        
        ServiceHost _serviceHost = null;

        /// <summary>
        /// System event logger
        /// </summary>
        private System.Diagnostics.EventLog _log;
      

        #endregion

        /// <summary>
        /// Service host
        /// </summary>
        public ServiceHost MyServiceHost
        {
            get { return _serviceHost; }
            set { _serviceHost = value; }
        }

        #region WCF

        /// <summary>
        /// Run wcf-service
        /// </summary>
        public virtual void StartHosting()
        {
            _log.WriteEntry("Service is running...");

            //service-host is ready to start
            if ((_serviceHost == null) ||
                (_serviceHost.State != CommunicationState.Opened))
            {
                //close just in case
                if (_serviceHost != null) StopHosting();

                try
                {
                    //create new host
                    _serviceHost = new ServiceHost(typeof(VfsService));
                    _serviceHost.Open();

                    _serviceHost.Faulted += new EventHandler(ServiceHost_Faulted);
                }
                catch (Exception ex)
                {
                    //if nothing helped, log and try again
                    _serviceHost = null;
                    if (ex.InnerException != null)
                        _log.WriteEntry(String.Format("Err = {0}\r\nInnerErr = {1}",
                            ex.Message, ex.InnerException.Message),System.Diagnostics.EventLogEntryType.Error);
                    else
                        _log.WriteEntry(String.Format("Err = {0}",
                            ex.Message),System.Diagnostics.EventLogEntryType.Error);

                    TryRestart();
                }
            }

        }

        /// <summary>
        /// Stop service
        /// </summary>
        public virtual void StopHosting()
        {
            _log.WriteEntry("Stopping service");

            //host is alive
            if (MyServiceHost != null)
            {
                //close it
                if (MyServiceHost.State != CommunicationState.Closed)
                {
                    if (MyServiceHost.State == CommunicationState.Faulted)
                        // tough
                        MyServiceHost.Abort();
                    else
                        // graceful
                        MyServiceHost.Close();

                }

                //can be collected
                MyServiceHost = null;
            }
        }

        /// <summary>
        /// restart service
        /// </summary>
        public virtual void ReStartHosting()
        {
            _log.WriteEntry("Service restart");

            StopHosting();
            StartHosting();
        }

        /// <summary>
        /// Handler for service host faults
        /// </summary>
        void ServiceHost_Faulted(object sender, EventArgs e)
        {
            _log.WriteEntry(String.Format("Service Error = {0}",
                e.ToString()), System.Diagnostics.EventLogEntryType.Error);

            TryRestart();
        }

        #endregion
        


        /// <summary>
        /// Manual start
        /// </summary>
        /// <param name="args"></param>
        public void ManualStart(string[] args)
        {
            OnStart(args);
        }

        /// <summary>
        /// Manual stop
        /// </summary>
        public void ManualStop()
        {
            OnStop();
        }

        public VfsWinService()
        { 
            InitializeComponent();
        
            _log.Source = "myVFS";
       
        }

        /// <summary>
        /// Windows service start
        /// </summary>
        protected override void OnStart(string[] args) 
        {
             StartHosting(); 
        }

        /// <summary>
        /// Windows service stop
        /// </summary>
        protected override void OnStop() { StopHosting(); }

        /// <summary>
        /// Initialization of windows service
        /// </summary>
        private void InitializeComponent()
        {
            this._log = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this._log)).BeginInit();
 
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.ServiceName = "myVFS";
            ((System.ComponentModel.ISupportInitialize)(this._log)).EndInit();

        }

        /// <summary>
        /// Service restart trial
        /// </summary>
        private void TryRestart()
        {
            if (this._nbRestartRequested < _serviceParameters.RetryNumberAuthorized)
            {
                this._nbRestartRequested += 1;

                ReStartHosting();
            }
            else
            {
                _log.WriteEntry("Number of service restarts exceeded. Service died.",
                    System.Diagnostics.EventLogEntryType.Error);
            }
        }

    }
}
