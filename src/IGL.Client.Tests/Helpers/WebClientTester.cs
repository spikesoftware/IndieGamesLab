using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace IGL.Client.Tests.Helpers
{
    internal class WebClientTester
    {
        public string Expected_Address { get; set; }
        public string Expected_Method { get; set; }        

        public byte[] UploadDataStringStringByteArray(string address, string method, byte[] data)
        {
            Assert.AreEqual(Expected_Address, address);
            Assert.AreEqual(Expected_Method, method);

            return new byte[0];
        }

        public byte[] UploadValuesStringNameValueCollection(string address, NameValueCollection collection)
        {
            // TODO: not hooked up yet

            return new byte[0];
        }
    }
}
