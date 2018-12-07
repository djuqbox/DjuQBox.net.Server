using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using NYoutubeDL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DjuQBox.net.Server
{
    class Program
    {
        //TODO change to configurations
        private const string MP3_PATH = @"C:\DjQbox\mp3s\test.test";
        private const string YOUTUBE_DL_PATH = @"C:\DjQbox\youtube-dl\youtube-dl.exe";
        private const string FFMPEG_PATH = @"C:\DjQbox\youtube-dl\ffmpeg-20170425-b4330a0-win64-static\bin";
        private const string GOOGLE_API_KEY = "AIzaSyBif1qH8cEFwWulm6KAPtwpDik4MvNBm5c"; //TODO change
        private const string MPC_PATH = @"C:\DjQbox\mpd\mpc-0.22\";
        private const string MPD_LIBRARY_DJUQBOX_ROOT_PATH = @"DJuQBox";

        //git proxy
        //https://stackoverflow.com/questions/44285651/set-proxy-for-microsoft-git-provider-in-visual-studio
        //https://stackoverflow.com/a/52557561/8289048
        //https://stackoverflow.com/a/14750116/8289048
        static void Main(string[] args)
        {

            string aPlayList = "PLSRDGXudTSm9PJfaMl2tq6I5DM--Gh8Is"; //args[0]

            PrepareYoutubeDL();

            var djuQBoxPlayList = GetYoutubePlayListInfo(aPlayList);
            
            DownloadPlayList(djuQBoxPlayList);



            //new Program().Run("test").Wait();

            Console.WriteLine("YouTube Data API: Search");
            Console.WriteLine("========================");

           

            String _q = Console.ReadLine();

            ///home/pi/hdd/mp3/DJuQBox

            //DownloadVideo("https://www.youtube.com/watch?v=7WFk23_6yos");

            //if (_q != String.Empty)
            //{
            //    new Program().Run(_q).Wait();
            //}



            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            MPC.MpcWrapper mpcWrapper = new MPC.MpcWrapper(MPC_PATH);
            
            mpcWrapper.Update(true, Path.Combine(MPD_LIBRARY_DJUQBOX_ROOT_PATH , aPlayList));
            mpcWrapper.PlaylistClear();
            mpcWrapper.PlaylistAddSong(Path.Combine(MPD_LIBRARY_DJUQBOX_ROOT_PATH, aPlayList));
            mpcWrapper.Play();


            //download
            //linux 
            //youtube -dl -x -i --proxy "" --audio-format mp3 --prefer-ffmpeg  https://www.youtube.com/playlist?list=PLSRDGXudTSm9PJfaMl2tq6I5DM--Gh8Is
            //windows 
            //youtube-dl.exe -x -i --proxy "http://ACSCOURIER\\hatizefstratiou:xxxxx@192.168.3.92:8080" --audio-format mp3 --prefer-ffmpeg  --ffmpeg-location "C:\DjQbox\youtube-dl\ffmpeg\bin" https://www.youtube.com/playlist?list=PLSRDGXudTSm9PJfaMl2tq6I5DM--Gh8Is
            //https://sourceforge.net/projects/cntlm/
            //http://cntlm.sourceforge.net/

            //https://www.youtube.com/watch?v=7WFk23_6yos

        }

        private static void PrepareYoutubeDL()
        {
            fYoutubeDL = new YoutubeDL();

            fYoutubeDL.StandardOutputEvent += (sender, output) => Console.WriteLine(output);
            fYoutubeDL.StandardErrorEvent += (sender, errorOutput) => Console.WriteLine(errorOutput);


            fYoutubeDL.Options.PostProcessingOptions.PreferFfmpeg = true;

            fYoutubeDL.Options.PostProcessingOptions.ExtractAudio = true;
            fYoutubeDL.Options.PostProcessingOptions.AudioFormat = NYoutubeDL.Helpers.Enums.AudioFormat.mp3;

            fYoutubeDL.Options.FilesystemOptions.Output = MP3_PATH;
            fYoutubeDL.YoutubeDlPath = YOUTUBE_DL_PATH;
            fYoutubeDL.Options.PostProcessingOptions.FfmpegLocation = FFMPEG_PATH;
        }

        private static void CreateFolder(DjuQBoxPlayList djuQBoxPlayList)
        {
            String _path = Path.Combine(MPD_LIBRARY_DJUQBOX_ROOT_PATH, djuQBoxPlayList.PlaylistId);
            if (Directory.Exists(_path))
            {
                //yparxei
            }
            else
            {
                Directory.CreateDirectory(_path);
            }
        }

        private static void DownloadPlayList(DjuQBoxPlayList djuQBoxPlayList)
        {
            CreateFolder(djuQBoxPlayList);

            fYoutubeDL.Options.FilesystemOptions.Output = Path.Combine(MP3_PATH, MPD_LIBRARY_DJUQBOX_ROOT_PATH, djuQBoxPlayList.PlaylistId) ;

            int ind = 1;
            foreach (var item in djuQBoxPlayList.Videos)
            {
                fYoutubeDL.VideoUrl = "https://www.youtube.com/watch?v=" + item.VideoId;
                fYoutubeDL.Options.FilesystemOptions.Output =
                    Path.Combine(MP3_PATH, MPD_LIBRARY_DJUQBOX_ROOT_PATH, djuQBoxPlayList.PlaylistId) + ind.ToString("n000") + "%(title)s-%(id)s.%(ext)s";
                 
                fYoutubeDL.Download();
            }

          

            //string commandToRun = fYoutubeDL.PrepareDownload();
            //Console.WriteLine(commandToRun);
            //https://gitlab.com/BrianAllred/NYoutubeDL
            //.\youtube-dl.exe -v -x -i --proxy "http://localhost:3128" --audio-format mp3 --prefer-ffmpeg  --ffmpeg-location "C:\DjQbox\youtube-dl\ffmpeg-20170425-b4330a0-win64-static\bin" https://www.youtube.com/watch?v=7WFk23_6yos


        

        }

        private static DjuQBoxPlayList GetYoutubePlayListInfo(string aPlayList)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = GOOGLE_API_KEY,
                ApplicationName = "DjuQBox.Net.Server"
            });

            var playlistSearch = youtubeService.Playlists.List("snippet,contentDetails");
            playlistSearch.Id = aPlayList;
            var playlist = playlistSearch.Execute();

          

            DjuQBoxPlayList list = new DjuQBoxPlayList();
            list.PlaylistId = aPlayList;
            list.Title = playlist.Items?[0].Snippet.Title;
            var _th = playlist.Items?[0].Snippet.Thumbnails.High; //an kenh>???
            list.Thumbnail = new Thumbnail()
            {
                Height = _th.Height,
                Width = _th.Width,
                Url = _th.Url,
            };
            list.SongCount = (int)playlist.Items?[0].ContentDetails?.ItemCount;

            var listSearch = youtubeService.PlaylistItems.List("snippet");
            listSearch.PlaylistId = aPlayList;
            listSearch.MaxResults = 50; // max value!!!!

            var nextPageToken = "";

            while (nextPageToken != null)
            {
                listSearch.PageToken = nextPageToken;
                var listSearchRes = listSearch.Execute();

                foreach (var listItem in listSearchRes.Items)
                {
                    list.Videos.Add(new DjuQBoxSong(listItem.Snippet.Title, listItem.Snippet.ResourceId.VideoId, listItem.Snippet.Thumbnails?.High?.Url));                       
                }

                nextPageToken = listSearchRes.NextPageToken;
            }


            return list;
        }

        static YoutubeDL fYoutubeDL;

        

        private static void DownloadVideo(string aVideoId)
        {
            fYoutubeDL.VideoUrl = "https://www.youtube.com/watch?v=7WFk23_6yos";

            //string commandToRun = fYoutubeDL.PrepareDownload();
            //Console.WriteLine(commandToRun);
            //https://gitlab.com/BrianAllred/NYoutubeDL
            //.\youtube-dl.exe -v -x -i --proxy "http://localhost:3128" --audio-format mp3 --prefer-ffmpeg  --ffmpeg-location "C:\DjQbox\youtube-dl\ffmpeg-20170425-b4330a0-win64-static\bin" https://www.youtube.com/watch?v=7WFk23_6yos


            fYoutubeDL.Download();
        }

        private async Task Run(String aQuery)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = GOOGLE_API_KEY,
                ApplicationName = this.GetType().ToString()
            });

            //var listSearch = youtubeService.PlaylistItems.List("id,contentDetails,snippet");
            var listSearch = youtubeService.PlaylistItems.List("snippet");
            listSearch.PlaylistId = "PLSRDGXudTSm9PJfaMl2tq6I5DM--Gh8Is";

            //listSearch.Fields = "items(snippet(resourceId(videoId),publishedAt,title,thumbnails/default/url))";
            listSearch.MaxResults = 50; // max value!!!!
            //listSearch.PageToken

            List<string> playListVideos = new List<string>();

            var playlistSearch = youtubeService.Playlists.List("snippet,contentDetails");
            playlistSearch.Id = "PLSRDGXudTSm9PJfaMl2tq6I5DM--Gh8Is";
            var playlist = playlistSearch.Execute();

            String _playlistTitle = playlist.Items?[0].Snippet.Title;
            String _playlistThumbUrl = playlist.Items?[0].Snippet.Thumbnails.High.Url;
            int _playlistCount = (int)playlist.Items?[0].ContentDetails?.ItemCount;

            var nextPageToken = "";

            while (nextPageToken != null)
            {
                listSearch.PageToken = nextPageToken;
                var listSearchRes = listSearch.Execute();

                foreach (var listItem in listSearchRes.Items)
                {
                    playListVideos.Add(String.Format("{0} ({1}) url: {2}", listItem.Snippet.Title, listItem.Snippet.ResourceId.VideoId, listItem.Snippet.Thumbnails?.High?.Url));
                }

                nextPageToken = listSearchRes.NextPageToken;
            }

            Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", playListVideos)));

            //var searchListRequest = youtubeService.Search.List("snippet");
            //searchListRequest.Q = aQuery; // Replace with your search term.
            //searchListRequest.MaxResults = 100;

            //// Call the search.list method to retrieve results matching the specified query term.
            //var searchListResponse = await searchListRequest.ExecuteAsync();

            //List<string> videos = new List<string>();
            //List<string> channels = new List<string>();
            //List<string> playlists = new List<string>();

            //// Add each result to the appropriate list, and then display the lists of
            //// matching videos, channels, and playlists.
            //foreach (var searchResult in searchListResponse.Items)
            //{
            //    switch (searchResult.Id.Kind)
            //    {
            //        case "youtube#video":
            //            videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
            //            break;

            //        case "youtube#channel":
            //            channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
            //            break;

            //        case "youtube#playlist":
            //            playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
            //            break;
            //    }
            //}

            //Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
            //Console.WriteLine(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
            //Console.WriteLine(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists)));
        }
    }
}
