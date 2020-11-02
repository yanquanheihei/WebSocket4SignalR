using log4net;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Extensions.MonoHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using WebSocket4Net;

namespace WebSocketClient4SignalR
{
    public class WebSocketClient
    {
        private static ILog logger = LogManager.GetLogger(typeof(WebSocketClient));

        private Timer reconnectTimer;

        private WebSocket webSocket;
        private RestClient restClient;

        /// <summary>
        /// Server host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// server port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// HTTP baseurl（readonly）
        /// </summary>
        public string HttpBaseUrl
        {
            get { return string.Format("http://{0}:{1}", this.Host, this.Port); }
        }

        /// <summary>
        /// WebSocket base url（readonly）
        /// </summary>
        public string WebSocketBaseUrl
        {
            get { return string.Format("ws://{0}:{1}", this.Host, this.Port); }
        }

        /// <summary>
        /// Subcribe method args
        /// </summary>
        public List<ConnectionData> ConnectionData { get; set; }

        /// <summary>
        /// Negotiation content
        /// </summary>
        public WSNegotiation WSNegotiation { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">server host</param>
        /// <param name="port">server port</param>
        public WebSocketClient(string host, int port)
        {
            this.Host = host;
            this.Port = port;

            this.restClient = new RestClient(this.HttpBaseUrl);
        }

        /// <summary>
        /// Connection established
        /// </summary>
        public event Action OnOpened;

        /// <summary>
        /// Connection disconnect
        /// </summary>
        public event Action OnClosed;

        /// <summary>
        /// Message received
        /// </summary>
        public event Action<string> OnMessageReceived;

        /// <summary>
        /// Connect to the server
        /// </summary>
        public void Open()
        {
            WSNegotiation negotiation = this.Negotiate();

            if (!string.IsNullOrEmpty(negotiation.ConnectionToken))
            {
                this.Connect(negotiation);
            }
            else
            {
                this.StartConnectTimer();
            }
        }

        /// <summary>
        /// DisConnect to the server
        /// </summary>
        public void Close()
        {
            IRestRequest request = new RestRequest("signalr/abort", Method.GET);

            string token = HttpUtility.UrlEncode(this.WSNegotiation.ConnectionToken);
            string connectionDatasJson = JsonConvert.SerializeObject(this.ConnectionData);
            request.AddQueryParameter("transport", "webSockets");
            request.AddQueryParameter("clientProtocol", "2.1");
            request.AddQueryParameter("connectionToken", token);
            request.AddQueryParameter("connectionData", connectionDatasJson);

            IRestResponse response = restClient.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
            }

            this.StopConnectTimer();
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message"></param>
        public void Send(Message message)
        {
            string msg = message.ToString();
            this.webSocket.Send(msg);
        }

        #region ReConnect timer

        private void StartConnectTimer()
        {
            if (this.reconnectTimer == null)
            {
                this.reconnectTimer = new System.Timers.Timer(10 * 1000);
                this.reconnectTimer.Elapsed += this.Timer_Elapsed;
            }

            this.reconnectTimer.AutoReset = true;
            this.reconnectTimer.Enabled = true;
            this.reconnectTimer.Start();
        }

        private void StopConnectTimer()
        {
            if (this.reconnectTimer != null)
            {
                this.reconnectTimer.Stop();
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Open();
        }

        #endregion ReConnect timer

        // 1、Content negotiation
        private WSNegotiation Negotiate()
        {
            WSNegotiation negotiation = new WSNegotiation();

            IRestRequest request = new RestRequest("signalr/negotiate", Method.GET);

            string connectionDatasJson = JsonConvert.SerializeObject(this.ConnectionData);
            request.AddQueryParameter("connectionData", connectionDatasJson);
            request.AddQueryParameter("clientProtocol", "2.1");

            IRestResponse response = restClient.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (!string.IsNullOrEmpty(response.Content))
                {
                    negotiation = JsonConvert.DeserializeObject<WSNegotiation>(response.Content);
                }
            }

            this.WSNegotiation = negotiation;

            return negotiation;
        }

        // 2、Create websocket connection
        private void Connect(WSNegotiation negotiation)
        {
            string token = HttpUtility.UrlEncode(negotiation.ConnectionToken);
            string connectionDatasJson = JsonConvert.SerializeObject(this.ConnectionData);
            string path = string.Format(@"{0}/signalr/connect?transport=webSockets&clientProtocol={1}&connectionToken={2}&connectionData={3}",
             this.WebSocketBaseUrl, negotiation.ProtocolVersion, token, connectionDatasJson);

            webSocket = new WebSocket(path);
            webSocket.Opened += WebSocket_Opened;
            webSocket.Closed += WebSocket_Closed;
            webSocket.MessageReceived += WebSocket_MessageReceived;
            webSocket.Open();
        }

        private void WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (this.OnMessageReceived != null)
            {
                this.OnMessageReceived(e.Message);
            }
        }

        private void WebSocket_Closed(object sender, EventArgs e)
        {
            if (this.OnClosed != null)
            {
                this.OnClosed();
            }

            this.webSocket.Dispose();

            this.StartConnectTimer();
        }

        private void WebSocket_Opened(object sender, EventArgs e)
        {
            this.StopConnectTimer();

            if (this.OnOpened != null)
            {
                this.OnOpened();
            }
        }
    }
}