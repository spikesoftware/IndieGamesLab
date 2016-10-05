
using System.Configuration;

namespace IGL.Client
{
    public static class Configuration
    {
        public static string ServiceNamespace = ConfigurationManager.AppSettings["AzureStorageConnectionString"];

        public static string IssuerName = ConfigurationManager.AppSettings["IssuerName"];
        public static string IssuerSecret = ConfigurationManager.AppSettings["IssuerSecret"];

        internal const string ACSHostName = "accesscontrol.windows.net";
        internal const string SBHostName = "servicebus.windows.net";

        internal const string QUEUE_GAMEEVENTS = "gameevents";
        internal const string QUEUE_PLAYEREVENTS = "playerevents";

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
