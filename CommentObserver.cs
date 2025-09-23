using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YoutubeCommentAPI
{
    internal class CommentObserver : IObserver<string>
    {
        private readonly string name;
        private readonly HttpListenerResponse _response;

        public CommentObserver(string n, HttpListenerResponse res)
        {
            this.name = n;
            this._response = res;
        }

        public void OnNext(string stat)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(stat);
            _response.ContentLength64 = buffer.Length;
            _response.ContentType = "text/plain; charset=utf-8";
            _response.OutputStream.Write(buffer, 0, buffer.Length);
            _response.OutputStream.Flush();
            Logger.Log($"{name}: Data successfully fetched and sent to the client.");
        }
        public void OnError(Exception e)
        {
            Logger.Log($"Error in comment stream: {e.Message}");
        }

        public void OnCompleted()
        {
            _response.OutputStream.Close();
            Logger.Log($"Data succsessfully fetched");
        }
    }
}
