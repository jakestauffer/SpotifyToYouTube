using SpotifyToYouTube.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyToYouTube.MusicClients.Abstractions
{
    public interface IMusicClient
    {
        public Task<List<Track>> GetTracksAsync(string playlistLink);

        public Task<Track> SearchAsync(string query);
    }
}
