
using System.Configuration;

namespace IGL.Client
{
    public static class Configuration
    {
        public static string ServiceNamespace = ConfigurationManager.AppSettings["AzureStorageConnectionString"];
        public static string IssuerName = ConfigurationManager.AppSettings["IssuerName"];
        public static string IssuerSecret = ConfigurationManager.AppSettings["IssuerSecret"];

        /// <summary>
        /// A global default for GameId for all GamePackets
        /// </summary>
        public static int GameId = 0;

        /// <summary>
        /// A global default for PlayerId for all GamePackets
        /// </summary>
        public static string PlayerId = "";

        internal const string ACSHostName = "accesscontrol.windows.net";
        internal const string SBHostName = "servicebus.windows.net";

        internal static string GetServiceMessagesAddress(string queue)
        {
            return string.Format("https://{0}.{1}/{2}/messages", ServiceNamespace, SBHostName, queue);
        }

        internal static string GetServiceSubscriptionsAddress(string queue, string subscription)
        {
            return string.Format("https://{0}.{1}/{2}/subscriptions/{3}/messages/head?timeout=60", ServiceNamespace, SBHostName, queue, subscription);
        }
    }
}
