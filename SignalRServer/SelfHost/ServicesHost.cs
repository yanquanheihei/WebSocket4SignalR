using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SignalRServer
{
    internal class ServicesHost
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(ServicesHost));

        public IEnumerable<IService> ServiceList { get; private set; }

        public ServicesHost()
        {
            this.ServiceList = new List<IService>()
            {
                new SelfHostService("Test", ConfigHelper.Instance.ExternalPort),
                LocalService.Instance
            };
        }

        public void Start()
        {
            foreach (var service in this.ServiceList)
            {
                try
                {
                    service.GetType().InvokeMember(
                        "Start",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod,
                        null,
                        service,
                        null);
                }
                catch (Exception ex)
                {
                    this.logger.Error(service.GetType().Name + " started error!", ex);
                }
            }
        }

        public void Stop()
        {
            foreach (var service in this.ServiceList)
            {
                try
                {
                    service.GetType().InvokeMember(
                        "Stop",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod,
                        null,
                        service,
                        null);
                }
                catch (Exception ex)
                {
                    this.logger.Error(service.GetType().Name + " stoped error!", ex);
                }
            }
        }
    }
}