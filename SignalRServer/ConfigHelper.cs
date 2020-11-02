using System.Configuration;

namespace SignalRServer
{
    public class ConfigHelper
    {
        private static readonly ConfigHelper instance = new ConfigHelper();

        public string ServiceName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public int ExternalPort { get; set; }

        private ConfigHelper()
        {
        }

        public static ConfigHelper Instance
        {
            get { return instance; }
        }

        static ConfigHelper()
        {
            instance.ServiceName = ConfigurationManager.AppSettings["ServiceName"];
            instance.DisplayName = ConfigurationManager.AppSettings["DisplayName"];
            instance.Description = ConfigurationManager.AppSettings["Description"];

            instance.ExternalPort = int.Parse(ConfigurationManager.AppSettings["ExternalPort"]);
        }
    }
}