using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCommentAPI
{
    internal class URLSettings
    {
        private readonly string apiKey = "AIzaSyAf6mjDCUSfhf7RvT0GfXxQuDGvfqboA30";
        private readonly int PortNum = 5050;
        private readonly string basicUrl = "https://www.googleapis.com/youtube/v3/commentThreads";

        public string GetURLPrefix()
        {
            return $"http://localhost:{PortNum}/";
        }

        public string GetURL(string videoId)
        {
            return $"{basicUrl}?part=snippet&videoId={videoId}&key={apiKey}";
        }
    }
}
