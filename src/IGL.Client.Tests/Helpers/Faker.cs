#if !DO_NOT_FAKE
using System.Collections.Specialized;
using System.Net.Fakes;
using System.Fakes;
using System.Text;
using System.IO;
using System.Threading;

namespace IGL.Client.Tests.Helpers
{
    public class Faker
    {
        public static void FakeOut()
        {
            var collection = new NameValueCollection();
            collection.Add("IGL.EncryptionSalt", "o6806642kbM7c5");
            collection.Add("AzureStorageConnectionString", "AzureStorageConnectionString");
            collection.Add("IssuerName", "IssuerName");
            collection.Add("IssuerSecret", "IssuerSecret");

            System.Configuration.Fakes.ShimConfigurationManager.AppSettingsGet = () => collection;

            //var responseShim = new ShimHttpWebResponse();
            ShimWebRequest.AllInstances.EndGetResponseIAsyncResult = (webrequest, ar) => new ShimHttpWebResponse
            {
                ResponseStreamGet = () => new MemoryStream(Encoding.ASCII.GetBytes("a response with a =somevalue&something"))
            };

            ShimHttpWebRequest shimWebRequest = new ShimHttpWebRequest
            {
                BeginGetResponseAsyncCallbackObject = (ac, o) =>
                {
                    // put in a delay
                    Thread.CurrentThread.Join(20);

                    RequestState rs = o as RequestState;

                    var stub = new StubIAsyncResult();

                    stub.AsyncStateGet = () => rs;

                    ac.Invoke(stub);

                    return stub;
                },
                EndGetResponseIAsyncResult = (ar) => new ShimHttpWebResponse
                {
                    ResponseStreamGet = () => new MemoryStream(Encoding.ASCII.GetBytes("a response with a =somevalue&something")),
                    GetResponseStream = () => new MemoryStream(Encoding.ASCII.GetBytes(DatacontractSerializerHelper.Serialize<GamePacket>(new GamePacket())))
                },                 
                AddressGet = () => new System.Uri("http://mytest"),
                HeadersGet = () => new System.Net.WebHeaderCollection()
            };


            ShimWebRequest.CreateString = (uri) => shimWebRequest;
            ShimWebRequest.CreateUri = (uri) => shimWebRequest;

            //requestShim.GetRequestStream = () => new MemoryStream();
            //requestShim.GetResponse = () => responseShim.Instance;
            // responseShim.GetResponseStream = () => new MemoryStream(Encoding.ASCII.GetBytes("Hello World"));

            ShimWebClient.AllInstances.UploadValuesStringNameValueCollection = (endpoint, values, b) =>
            {
                // put in a delay
                Thread.CurrentThread.Join(20);
           
                return Encoding.ASCII.GetBytes("a response with a =somevalue&something");
            };

            ShimWebClient.AllInstances.UploadDataStringStringByteArray = (address, method, values, response) =>
            {
                // put in a delay
                Thread.CurrentThread.Join(20);

                return Encoding.ASCII.GetBytes("everything is all good");
            };


        }
    }
}
#endif