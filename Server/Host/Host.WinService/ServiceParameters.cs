using System;
using System.Configuration;

namespace Host.WinService
{
    /// <summary>
    /// Parameters of the service
    /// </summary>
    internal class ServiceParameters
    {
        // defaults
        public const int DEFAULT_RETRYNUMBERAUTHORIZED = 3;
        private const string retryNumberAuthorizedSettingName = "retryNumberAuthorized";

        // number of reconnect attempts
        private int retryNumberAuthorized;

        /// <summary>
        /// number of reconnect attempts after failure
        /// </summary>
        public int RetryNumberAuthorized
        {
            get { return retryNumberAuthorized; }
            set { retryNumberAuthorized = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ServiceParameters()
        {
            UpdateConf();
        }

        /// <summary>
        /// read settings from app config
        /// </summary>
        public void UpdateConf()
        {
            this.retryNumberAuthorized = DEFAULT_RETRYNUMBERAUTHORIZED;

            String confValue = String.Empty;
            foreach (string aValue in ConfigurationManager.AppSettings)
            {
                switch (aValue)
                {
                    case retryNumberAuthorizedSettingName:
                        
                        confValue = ConfigurationManager.AppSettings[aValue];
                        if (String.IsNullOrEmpty(confValue) == false)
                        {
                            if (int.TryParse(confValue, out this.retryNumberAuthorized) == false)
                                this.retryNumberAuthorized = DEFAULT_RETRYNUMBERAUTHORIZED;
                        }
                        break;

                }
            }
        }
    }

}
