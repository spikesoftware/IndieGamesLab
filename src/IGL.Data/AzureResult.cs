using System.Collections.Generic;

namespace IGL.Data
{
    public class AzureResult
    {
        public bool WasSuccessful
        {
            get { return this.Code == 0; }
        }
        public int Code { get; set; }
        public string Message { get; set; }

        public static AzureResult GetSuccessResponse()
        {
            return new AzureResult { Code = 0 };
        }
    }

    public class AzureResult<T>
    {
        public bool WasSuccessful
        {
            get { return this.Code == 0; }
        }
        public int Code { get; set; }
        public string Message { get; set; }
        public T ResultObject { get; set; }
    }
}
