#if !DO_NOT_FAKE
using System.Collections.Specialized;


namespace IGL.Common.Tests.Helpers
{
    public class Faker
    {
        public static void FakeOut()
        {
            var collection = new NameValueCollection();            

            System.Configuration.Fakes.ShimConfigurationManager.AppSettingsGet = () => collection;
        }
    }
}
#endif