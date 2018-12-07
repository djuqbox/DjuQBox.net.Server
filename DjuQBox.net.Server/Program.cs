using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using NYoutubeDL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DjuQBox.net.Server
{
    class Program
    {
        //git proxy
        //https://stackoverflow.com/questions/44285651/set-proxy-for-microsoft-git-provider-in-visual-studio
        //https://stackoverflow.com/a/52557561/8289048
        //https://stackoverflow.com/a/14750116/8289048
        static void Main(string[] args)
        {

            new Program().Run("test").Wait();

            Console.WriteLine("YouTube Data API: Search");
            Console.WriteLine("========================");

            fYoutubeDL = new YoutubeDL();
           
            fYoutubeDL.StandardOutputEvent += (sender, output) => Console.WriteLine(output);
            fYoutubeDL.StandardErrorEvent += (sender, errorOutput) => Console.WriteLine(errorOutput);

            
            fYoutubeDL.Options.PostProcessingOptions.PreferFfmpeg = true;
            
            fYoutubeDL.Options.PostProcessingOptions.ExtractAudio = true;
            fYoutubeDL.Options.PostProcessingOptions.AudioFormat = NYoutubeDL.Helpers.Enums.AudioFormat.mp3;

            fYoutubeDL.Options.FilesystemOptions.Output = @"C:\DjQbox\mp3s\test.test";
            fYoutubeDL.YoutubeDlPath = @"C:\DjQbox\youtube-dl\youtube-dl.exe";
            fYoutubeDL.Options.PostProcessingOptions.FfmpegLocation = @"C:\DjQbox\youtube-dl\ffmpeg-20170425-b4330a0-win64-static\bin";

            String _q = Console.ReadLine();

            ///home/pi/hdd/mp3/DJuQBox

            DownloadVideo("https://www.youtube.com/watch?v=7WFk23_6yos");

            //if (_q != String.Empty)
            //{
            //    new Program().Run(_q).Wait();
            //}



            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            


            //download
            //linux 
            //youtube -dl -x -i --proxy "" --audio-format mp3 --prefer-ffmpeg  https://www.youtube.com/playlist?list=PLSRDGXudTSm9PJfaMl2tq6I5DM--Gh8Is
            //windows 
            //youtube-dl.exe -x -i --proxy "http://ACSCOURIER\\hatizefstratiou:xxxxx@192.168.3.92:8080" --audio-format mp3 --prefer-ffmpeg  --ffmpeg-location "C:\DjQbox\youtube-dl\ffmpeg\bin" https://www.youtube.com/playlist?list=PLSRDGXudTSm9PJfaMl2tq6I5DM--Gh8Is
            //https://sourceforge.net/projects/cntlm/
            //http://cntlm.sourceforge.net/

            //https://www.youtube.com/watch?v=7WFk23_6yos

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
                ApiKey = "AIzaSyBif1qH8cEFwWulm6KAPtwpDik4MvNBm5c",
                ApplicationName = this.GetType().ToString()
            });

            //var listSearch = youtubeService.PlaylistItems.List("id,contentDetails,snippet");
            var listSearch = youtubeService.PlaylistItems.List("snippet");
            listSearch.PlaylistId = "PLSRDGXudTSm9PJfaMl2tq6I5DM--Gh8Is";
            //listSearch.Key = "AIzaSyBif1qH8cEFwWulm6KAPtwpDik4MvNBm5c";
            //listSearch.Fields = "items(snippet(resourceId(videoId),publishedAt,title,thumbnails/default/url))";
            listSearch.MaxResults = 50; // max value!!!!
            //listSearch.PageToken

            List<string> playListVideos = new List<string>();

            var playlistSearch = youtubeService.Playlists.List("snippet");
            playlistSearch.Id = "PLSRDGXudTSm9PJfaMl2tq6I5DM--Gh8Is";
            var playlist = playlistSearch.Execute();

            String _playlistTitle = playlist.Items?[0].Snippet.Title;
            String _playlistThumbUrl = playlist.Items?[0].Snippet.Thumbnails.High.Url;

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
