using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeCommentAPI
{
    internal class WebServer
    { 
        
        //private readonly CommentObserver commentObserver;
       // private readonly CommentStream commentStream;
        
        private readonly URLSettings urlSettings;
        private IDisposable _subscription;
        private IObservable<HttpListenerContext> _observe;

        private readonly HttpListener _listener = new HttpListener();

       public WebServer(URLSettings url)
        {
            urlSettings = url;
            _listener.Prefixes.Add(urlSettings.GetURLPrefix());
           // commentObserver = co;
            //commentStream = cs;
        }

        public void Start()
        {
            _listener.Start();
            Logger.Log("Web server started");

            _observe = Observable
              .Defer(() => Observable.FromAsync(_listener.GetContextAsync))
              .Repeat();

            _subscription = _observe
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(async ctx =>
                {
                    try
                    {
                        RequestHandler(ctx);
                    }
                    catch(Exception err)
                    {
                        Logger.Log($"Error request handling: {err.Message}");
                    }
                },
                exception =>
                {
                    Logger.Log($"Stream error: {exception.Message}");
                }
                );
        }

        public async void RequestHandler(HttpListenerContext context)
        {
            HttpListenerRequest _request = context.Request;
            HttpListenerResponse _response = context.Response;

            string videoId = _request.QueryString["videoId"];

            if (!string.IsNullOrEmpty(videoId))
            {
                //ako string nije null ili empty, treba da vratimo rezultat klijentu:
                var commentService = new CommentService(urlSettings);
                var comments = await commentService.FetchCommentsAsync(videoId);
                Logger.Log($"Request: {_request.RawUrl}");
                var _obs = new CommentObserver(Thread.CurrentThread.Name, _response);

                int _numLikes = 0;
                int _numReply = 0;

                int reactions = 0;

                double _likes = 0;
                double _reply = 0;

                foreach(var c in comments)
                {
                    _numLikes += c.LikeCount;
                    _numReply += c.ReplyCount;

                }

                reactions = _numLikes + _numReply;
                if (reactions > 0)
                {
                    _likes = ((double)_numLikes / reactions) * 100;
                    _reply = ((double)_numReply / reactions) * 100;
                }
                else
                {
                    _likes = 0;
                    _reply = 0;
                }

                string res = $"Udeo lajkova u ukupnom broju komentara: {_likes}\n" +
                    $"Udeo odgovora u ukupnom broju komentara: {_reply}\n" +
                    $"Ukupan broj reakcija na komentare videa: {reactions}";

                _obs.OnNext(res);
                
                _obs.OnCompleted();
            }
            else
            {
                string clientResponse = $"Video with this id does not exist: {videoId}";
                byte[] buffer = Encoding.UTF8.GetBytes(clientResponse);
                _response.ContentLength64 = buffer.Length;
                _response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            context.Response.OutputStream.Close();
        }
        public void Stop()
        {
            _subscription?.Dispose();
            _listener.Stop();
        }

    }
}
