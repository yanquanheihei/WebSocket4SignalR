using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Timers;
using TestType;
using WebSocketClient4SignalR;

namespace TestClient
{
    internal class Program
    {
        private static ILog logger = LogManager.GetLogger(typeof(Program));

        private static Timer reSendTimer;
        private static WebSocketClient client;

        private static void Main(string[] args)
        {
            client = new WebSocketClient("localhost", 8808);
            client.ConnectionData = new List<ConnectionData>
            {
                new ConnectionData { Name = "multipleClientsHub" }
            };

            client.OnOpened += Client_OnOpened;
            client.OnClosed += Client_OnClosed;
            client.OnMessageReceived += Client_OnMessageReceived;

            client.Open();

            Console.Read();
        }

        private static void Client_OnMessageReceived(string message)
        {
            ReceivedMessage receivedMessage = JsonConvert.DeserializeObject<ReceivedMessage>(message);

            if (receivedMessage != null && receivedMessage.Messages != null && receivedMessage.Messages.Count > 0)
            {
                foreach (var msg in receivedMessage.Messages)
                {
                    switch (msg.Method)
                    {
                        case "updateMsg":
                            TestMessage testMessage = JsonConvert.DeserializeObject<TestMessage>(msg.Values[0].ToString());
                            logger.InfoFormat("Received msg:{0}", testMessage.ToString());
                            break;

                        case "testMsg":
                            string ms = msg.Values[0].ToString();
                            logger.InfoFormat("Received msg:{0}", ms);
                            break;
                    }
                }
            }
        }

        private static void Client_OnClosed()
        {
            logger.Warn("DisConnected!");
        }

        private static void Client_OnOpened()
        {
            logger.Info("Connected!");

            StartSendTimer();
        }

        private static void StartSendTimer()
        {
            if (reSendTimer == null)
            {
                reSendTimer = new System.Timers.Timer(2 * 1000);
                reSendTimer.Elapsed += Timer_Elapsed;
            }

            reSendTimer.AutoReset = true;
            reSendTimer.Enabled = true;
            reSendTimer.Start();
        }

        private static void StopSendTimer()
        {
            if (reSendTimer != null)
            {
                reSendTimer.Stop();
            }
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TestMessage message = new TestMessage
            {
                Code = "110",
                Message = "Test msg"
            };

            List<object> values = new List<object>();
            values.Add(message);

            Message msg = new Message
            {
                HubName = "multipleClientsHub",
                Method = "sendTestMessage",
                Values = values
            };

            client.Send(msg);
        }
    }
}