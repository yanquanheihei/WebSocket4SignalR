using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using TestType;

namespace SignalRServer
{
    [HubName("multipleClientsHub")]
    public class MultipleClientsHub : Hub
    {
        private static ILog logger = LogManager.GetLogger(typeof(MultipleClientsHub));

        [HubMethodName("sendTestMessage")]
        public void SendTestMessage(TestMessage message)
        {
            logger.InfoFormat("Received message:{0}", message.ToString());
        }
    }
}