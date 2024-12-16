using Newtonsoft.Json.Linq;
using SpotifyToYouTube.Models;
using SpotifyToYouTube.MusicClients.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SpotifyToYouTube.MusicClients
{
    public class YouTube : BaseMusicClient
    {
        private string _apiKey;

        private YouTube(string apiKey)
        {
            _apiKey = apiKey;
        }

        public static YouTube CreateAuthorizedClient(string apiKey) => new(apiKey);

        public override async Task<List<Track>> GetTracksAsync(string playlistLink)
        {
            string? nextPageToken = null;

            var playlistId = ExtractPlaylistId(playlistLink);
            var tracks = new List<Track>();

            do
            {
                var url = $"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&playlistId={playlistId}&maxResults=50&key={_apiKey}";

                if (!string.IsNullOrWhiteSpace(nextPageToken))
                {
                    url += $"&pageToken={nextPageToken}";
                }

                var response = await HttpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {response.ReasonPhrase}");

                    return tracks;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(jsonResponse);

                foreach (var item in json["items"])
                {
                    var title = item["snippet"]["title"].ToString();
                    var videoId = item["snippet"]["resourceId"]["videoId"].ToString();
                    var videoUrl = $"https://www.youtube.com/watch?v={videoId}";

                    tracks.Add(new Track()
                    {
                        Title = title
                    });
                }

                nextPageToken = json["nextPageToken"]?.ToString();

            } while (!string.IsNullOrEmpty(nextPageToken));

            return tracks;
        }

        private static string ExtractPlaylistId(string url)
        {
            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                var query = HttpUtility.ParseQueryString(uri.Query);
                return query["list"]; // Returns the value of the 'list' parameter
            }
            return null;
        }

        public override async Task<Track> SearchAsync(string query)
        {
            var response =
                await HttpClient.GetStringAsync(
                 $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={Uri.EscapeDataString(query)}&key={_apiKey}&type=video");

            var youtubeData = JObject.Parse(response);
            var item = youtubeData["items"][0];

            return new Track()
            {
                Title = item["snippet"]["title"].ToString(),
                Link = $"https://www.youtube.com/watch?v={item["id"]["videoId"].ToString()}"
            };
        }
    }
}
