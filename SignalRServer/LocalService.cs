using log4net;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Timers;

namespace SignalRServer
{
    public class LocalService : IService
    {
        private static ILog logger = LogManager.GetLogger(typeof(LocalService));

        private static readonly LocalService instance = new LocalService();

        private IHubContext hubContext;

        private System.Timers.Timer timer;

        private LocalService()
        {
            this.hubContext = GlobalHost.ConnectionManager
                .GetHubContext<MultipleClientsHub>();
        }

        public static LocalService Instance
        {
            get { return instance; }
        }

        public void Start()
        {
            logger.Info("Localservice started");

            this.StartTimer();
        }

        public void Stop()
        {
            logger.Info("Localservice stopped");
        }

        private void StartTimer()
        {
            if (this.timer == null)
            {
                this.timer = new System.Timers.Timer(2000);
                this.timer.Elapsed += this.Timer_Elapsed;
            }

            this.timer.AutoReset = true;
            this.timer.Enabled = true;
            this.timer.Start();
        }

        private Random random = new Random();

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string msg = random.Next(0, 10000).ToString();
            MessageInfo messageInfo = new MessageInfo
            {
                Code = "901",
                Message = string.Format("Test data {0}", msg)
            };
            this.hubContext.Clients.All.updateMsg(messageInfo);

            this.hubContext.Clients.All.testMsg(msg);

            //logger.InfoFormat("Send msg: {0}", "Hello");

            //List<string> ids = ConnectionMapping<string>.Instanc
            //    .GetConnections("MainUI").ToList();

            //if (ids != null && ids.Count > 0)
            //{
            //    string id = ids[0];
            //    string msg = DateTime.Now.ToString();
            //    this.hubContext.Clients.Client(id).updateMsg(msg);

            //    Console.WriteLine("Send:{0}", msg);
            //}
        }
    }

    public class MessageInfo
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("length")]
        public int MessageLength { get { return Message.Length; } }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}