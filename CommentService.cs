using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using Newtonsoft.Json.Linq;


namespace YoutubeCommentAPI
{
    internal class CommentService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly URLSettings _urlSettings;

        public CommentService(URLSettings urlSettings)
        {
            _urlSettings = urlSettings;
        }
        public async Task<IEnumerable<Comment>> FetchCommentsAsync(string videoID)
        {
            var url = _urlSettings.GetURL(videoID);
            Logger.Log($"URL send to API: {url}");

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                var JsonResponse = JObject.Parse(json);
                var jsonComment = JsonResponse["items"];


                if(jsonComment == null)
                {
                    return Enumerable.Empty<Comment>();
                }
                //to do: popravi ovo da radi ono sto trazi zadatak, za sad samo vraca listu komentara i
                //specificirane zahteve.
                return jsonComment.Select(item => new Comment
                {
                    Text = (string)item["snippet"]?["topLevelComment"]?["snippet"]?["textDisplay"],

                    AuthorName = (string)item["snippet"]?["topLevelComment"]?["snippet"]?["authorDisplayName"],

                    LikeCount = item["snippet"]?["topLevelComment"]?["snippet"]?["likeCount"]?.Value<int>() ?? 0,

                    ReplyCount = item["snippet"]?["totalReplyCount"]?.Value<int>() ?? 0,
                });
            }
            catch(Exception err)
            {
                Logger.Log($"Error while executing fecth: {err.Message}");
                return Enumerable.Empty<Comment>();
            }
        }
    }
}
