using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace IGL.Client
{

    /// <summary>
    /// ServiceBusListener will poll for events from the IGL services
    /// </summary>
    public class ServiceBusListener : ServiceBusBase, IDisposable
    {
        static bool _shouldRun = false;
        static bool _isRunning = false;

        public static string PlayerId;

        Thread _thread = null;

        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;

        public void StartListening()
        {
            if (string.IsNullOrEmpty(PlayerId))
                throw new ApplicationException("ServiceBuslistener.StartListening() PlayerId must be set before listening for events.");

            _shouldRun = true;

            _thread = new Thread(new ThreadStart(ServiceBusListenerThread));
            _thread.Start();
        }

        public void StopListening()
        {
            _shouldRun = false;
        }

        /// <summary>
        /// Event called when a GameEvent has been successfully received.
        /// </summary>
        public static event EventHandler<GamePacketArgs> OnGameEventReceived;
        public static event EventHandler<ErrorEventArgs> OnListenError;

        /// <summary>
        /// receive an event targeted to this particular player
        /// </summary>
        /// <param name="post"></param>
        static void ServiceBusListenerThread() 
        {
            _isRunning = true;
            
            var address = new Uri(Configuration.GetServiceSubscriptionsAddress(Configuration.QUEUE_PLAYEREVENTS, PlayerId));
            while (_shouldRun)
            {
                try
                {
                    WebRequest request = WebRequest.Create(address);
                    request.Headers[HttpRequestHeader.Authorization] = GetToken();
                    request.Method = "DELETE";
                    RequestState rs = new RequestState();
                    rs.Request = request;

                    IAsyncResult r = request.BeginGetResponse(new AsyncCallback(RespCallback), rs);

                    allDone.WaitOne();
                }
                catch(Exception ex)
                {
                    OnListenError?.Invoke(null, new ErrorEventArgs(ex));                    
                }
            }
                        
            _isRunning = false;
        }

        private static void RespCallback(IAsyncResult ar)
        {
            try
            {
                // Get the RequestState object from the async result.
                RequestState rs = (RequestState)ar.AsyncState;

                // Get the WebRequest from RequestState.
                WebRequest req = rs.Request;

                // Call EndGetResponse, which produces the WebResponse object
                //  that came from the request issued above.
                WebResponse resp = req.EndGetResponse(ar);

                //  Start reading data from the response stream.
                Stream ResponseStream = resp.GetResponseStream();

                // Store the response stream in RequestState to read 
                // the stream asynchronously.
                rs.ResponseStream = ResponseStream;

                //  Pass rs.BufferRead to BeginRead. Read data into rs.BufferRead
                IAsyncResult iarRead = ResponseStream.BeginRead(rs.BufferRead, 0,
                   BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);
            }
            catch(Exception ex)
            {
                OnListenError?.Invoke(null, new ErrorEventArgs(ex));
            }
        }

        static void ReadCallBack(IAsyncResult asyncResult)
        {
            try
            {
                // Get the RequestState object from AsyncResult.
                RequestState rs = (RequestState)asyncResult.AsyncState;

                // Retrieve the ResponseStream that was set in RespCallback. 
                Stream responseStream = rs.ResponseStream;

                // Read rs.BufferRead to verify that it contains data. 
                int read = responseStream.EndRead(asyncResult);
                if (read > 0)
                {
                    // Prepare a Char array buffer for converting to Unicode.
                    Char[] charBuffer = new Char[BUFFER_SIZE];

                    // Convert byte stream to Char array and then to String.
                    // len contains the number of characters converted to Unicode.
                    int len =
                       rs.StreamDecode.GetChars(rs.BufferRead, 0, read, charBuffer, 0);

                    String str = new String(charBuffer, 0, len);

                    // Append the recently read data to the RequestData stringbuilder
                    // object contained in RequestState.
                    rs.RequestData.Append(
                       Encoding.ASCII.GetString(rs.BufferRead, 0, read));

                    // Continue reading data until 
                    // responseStream.EndRead returns –1.
                    IAsyncResult ar = responseStream.BeginRead(
                       rs.BufferRead, 0, BUFFER_SIZE,
                       new AsyncCallback(ReadCallBack), rs);

                    HandlePacket(str);
                }
                else
                {
                    if (rs.RequestData.Length > 0)
                    {
                        HandlePacket(rs.RequestData.ToString());
                    }
                    // Close down the response stream.
                    responseStream.Close();
                    // Set the ManualResetEvent so the main thread can exit.
                    allDone.Set();
                }
            }
            catch (Exception ex)
            {
                OnListenError?.Invoke(null, new ErrorEventArgs(ex));
            }

            return;
        }

        static void HandlePacket(string message)
        {
            var packet = DatacontractSerializerHelper.Deserialize<GamePacket>(message);

            if (packet != null)
            {
                OnGameEventReceived?.Invoke(null, new GamePacketArgs { GamePacket = packet });
            }
        }

        public void Dispose()
        {
            // close down the listening
            _shouldRun = false;

            while(_isRunning)
            {
                Thread.Sleep(200);
            }
        }
    }        
}
