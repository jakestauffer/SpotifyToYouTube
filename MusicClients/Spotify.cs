using Newtonsoft.Json.Linq;
using SpotifyToYouTube.Models;
using SpotifyToYouTube.MusicClients.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SpotifyToYouTube.MusicClients
{
    public class Spotify : BaseMusicClient
    {
        private Spotify(string accessToken)
        {
            HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        }

        public static async Task<Spotify> CreateAuthorizedClientAsync(string clientId, string clientSecret)
        {
            using var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));
            request.Content =
                new FormUrlEncodedContent(
                    [
                        new KeyValuePair<string, string>("grant_type", "client_credentials")
                    ]);

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Invalid Spotify credentials");
            }

            var content = await response.Content.ReadAsStringAsync();
            var tokenData = JObject.Parse(content);

            var accessToken = tokenData["access_token"]!.ToString();

            return new Spotify(accessToken);
        }

        public override async Task<List<Track>> GetTracksAsync(string playlistLink)
        {
            var playlistId = 
                playlistLink.Split('/')
                            .ElementAtOrDefault(4)
                            ?.Split('?')
                            ?.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(playlistId))
            {
                throw new Exception("Invalid Spotify playlist link");
            }

            var response = 
                await HttpClient.GetStringAsync($"https://api.spotify.com/v1/playlists/{playlistId}/tracks");

            var playlistData = JObject.Parse(response);

            var tracks = new List<Track>();
            foreach (var item in playlistData["items"])
            {
                var trackName = item["track"]["name"].ToString();
                var artistName = item["track"]["artists"][0]["name"].ToString();

                tracks.Add(new Track()
                {
                    Title = $"{trackName} by {artistName}"
                });
            }

            return tracks;
        }

        public override async Task<Track> SearchAsync(string query)
        {
            var url = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=track&limit=10";

            var response = await HttpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to search Spotify. Error: {response.ReasonPhrase}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(jsonResponse);

            var item = json["tracks"]["items"][0];

            var trackName = item["name"].ToString();
            var artistName = item["artists"][0]["name"].ToString();

            return new Track()
            {
                Title = $"{trackName} by {artistName}",
                Link = item["external_urls"]["spotify"].ToString()
            };
        }
    }
}
