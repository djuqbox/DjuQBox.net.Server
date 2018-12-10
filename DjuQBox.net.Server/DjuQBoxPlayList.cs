

using System.Collections.Generic;

namespace DjuQBox.net.Server
{
    public class DjuQBoxPlayList
    {
        public DjuQBoxPlayList()
        {
            Videos = new List<DjuQBoxSong>();
        }

        public string PlaylistId { get; internal set; }
        public string Title { get; internal set; }
        public string ThumbnailUrl { get; internal set; }
        public int SongCount { get; internal set; }
        public List<DjuQBoxSong> Videos { get; internal set; }
    }

    public class DjuQBoxSong
    {


        public DjuQBoxSong(string title, string videoId, string url)
        {
            this.Title = title;
            this.VideoId = videoId;
            this.ThumbnailUrl = url;
        }

        public string Title { get; internal set; }
        public string VideoId { get; internal set; }
        public string ThumbnailUrl { get; internal set; }
    }
    
}