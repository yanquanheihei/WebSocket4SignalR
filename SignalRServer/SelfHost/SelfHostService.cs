using log4net;
using Microsoft.Owin.Hosting;
using System;

namespace SignalRServer
{
    internal class SelfHostService : IService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SelfHostService));
        private IDisposable serverDisposable;

        public string ServiceType { get; private set; }

        public int ServiceHostPort { get; private set; }

        public SelfHostService(string serviceType, int serviceHostPort)
        {
            this.ServiceType = serviceType;
            this.ServiceHostPort = serviceHostPort;
        }

        public void Start()
        {
            string host = string.Format("http://*:{0}", this.ServiceHostPort);

            try
            {
                this.serverDisposable = WebApp.Start(host);
                logger.InfoFormat("{0} service started:{1}", this.ServiceType, host);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("{0} service started failed:{1},{2}", this.ServiceType, host, ex);
            }
        }

        public void Stop()
        {
            // 释放服务资源
            this.serverDisposable.Dispose();
        }
    }
}