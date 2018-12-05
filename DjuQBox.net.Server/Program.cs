using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
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
            Console.WriteLine("YouTube Data API: Search");
            Console.WriteLine("========================");

            String _q = Console.ReadLine();
            if (_q != String.Empty)
            {
                new Program().Run(_q).Wait();
            }

            

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();


            //download
            //linux 
            //youtube -dl -x -i --proxy "" --audio-format mp3 --prefer-ffmpeg  https://www.youtube.com/playlist?list=PLSRDGXudTSm9PJfaMl2tq6I5DM--Gh8Is
            //windows 
            //youtube-dl.exe -x -i --proxy "http://ACSCOURIER\\hatizefstratiou:xxxxx@192.168.3.92:8080" --audio-format mp3 --prefer-ffmpeg  --ffmpeg-location "C:\DjQbox\youtube-dl\ffmpeg\bin" https://www.youtube.com/playlist?list=PLSRDGXudTSm9PJfaMl2tq6I5DM--Gh8Is
        }

        private async Task Run(String aQuery)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyBif1qH8cEFwWulm6KAPtwpDik4MvNBm5c",
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = aQuery; // Replace with your search term.
            searchListRequest.MaxResults = 50;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<string> videos = new List<string>();
            List<string> channels = new List<string>();
            List<string> playlists = new List<string>();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                        break;

                    case "youtube#channel":
                        channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                        break;

                    case "youtube#playlist":
                        playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                        break;
                }
            }

            Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
            Console.WriteLine(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
            Console.WriteLine(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists)));
        }
    }
}
