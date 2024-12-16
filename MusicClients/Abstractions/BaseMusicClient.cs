using SpotifyToYouTube.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyToYouTube.MusicClients.Abstractions
{
    public abstract class BaseMusicClient : IMusicClient
    {
        protected HttpClient HttpClient;

        public BaseMusicClient() => HttpClient = new HttpClient();

        public abstract Task<List<Track>> GetTracksAsync(string playlistLink);

        public abstract Task<Track> SearchAsync(string query);
    }
}
