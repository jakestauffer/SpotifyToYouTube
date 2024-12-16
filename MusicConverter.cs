using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyToYouTube.Models;
using SpotifyToYouTube.MusicClients.Abstractions;

namespace SpotifyToYouTube
{
    public class MusicConverter
    {
        private readonly IMusicClient _source;
        private readonly IMusicClient _destination;

        public MusicConverter(IMusicClient source, IMusicClient destination) 
        {
            _source = source;
            _destination = destination;
        }

        public async Task<List<Track>> ConvertAsync(string sourcePlaylistLink)
        {
            Console.WriteLine("Starting search ...");

            var sourceTracks = await _source.GetTracksAsync(sourcePlaylistLink);

            Console.WriteLine("\nFound these tracks in the source playlist: ");
            Console.WriteLine(string.Join("\n", sourceTracks));

            Console.WriteLine("\nStarting to convert ...");

            var resultTracks = new List<Track>();
            foreach (var sourceTrack in sourceTracks)
            {
                resultTracks.Add(await _destination.SearchAsync(sourceTrack.Title));
            }

            Console.WriteLine("\nDone converting!");

            return resultTracks;
        }
    }
}
