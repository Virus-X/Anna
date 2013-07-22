using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using Anna.Request;

namespace Anna.Responses
{
    public class Response
    {
        protected HttpListenerResponse ListenerResponse { get; private set; }

        public Response(RequestContext context, int statusCode = 200)
        {
            StatusCode = statusCode;
            Headers = new Dictionary<string, string>();
            ContentType = "text/html";
            ListenerResponse = context.ListenerResponse;
        }

        public int StatusCode { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public string ContentType
        {
            get
            {
                return Headers.ContainsKey("Content-Type") ? Headers["Content-Type"] : null;
            }

            set
            {
                Headers["Content-Type"] = value;
            }
        }

        public virtual IObservable<Stream> WriteStream(Stream s)
        {
            return Observable.Return(s);
        }

        public virtual void Send()
        {
            foreach (var header in Headers.Where(r => r.Key != "Content-Type"))
            {
                ListenerResponse.AddHeader(header.Key, header.Value);
            }

            ListenerResponse.ContentType = ContentType;
            ListenerResponse.StatusCode = StatusCode;
            WriteStream(ListenerResponse.OutputStream)
                        .Subscribe(s =>
                        {
                            try
                            {
                                s.Close();
                            }
                            catch (HttpListenerException e)
                            {
                                // 1229 = client closed connection.
                                if (e.ErrorCode != 1229) throw;
                            }
                            finally
                            {
                                s.Dispose();
                            }
                        }, e =>
                        {
                            try
                            {
                                ListenerResponse.StatusCode = 500;
                                ListenerResponse.OutputStream.Close();
                            }
                            catch { } //swallow exceptions
                        });    
            
        }
    }
}