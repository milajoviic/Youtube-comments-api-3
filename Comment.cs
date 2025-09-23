using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCommentAPI
{
    internal class Comment
    {
        public string AuthorName { get; set; }
        public string Text { get; set; }
        public int LikeCount { get; set; }
        public DateTime PublishedDate { get; set; }

        public int ReplyCount { get; set; }
    }
}
