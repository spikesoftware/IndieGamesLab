#if !FAKES_NOT_SUPPORTED
using System.Collections.Specialized;


namespace IGL.Common.Tests.Helpers
{
    public class Faker
    {
        public static void FakeOut()
        {
            var collection = new NameValueCollection();
            collection.Add("IGL.EncryptionSalt", "o6806642kbM7c5");

            System.Configuration.Fakes.ShimConfigurationManager.AppSettingsGet = () => collection;
        }
    }
}
#endif