using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyToYouTube.Models
{
    public class Track
    {
        public string Title { get; set; } = null!;

        public string? Link { get; set; }

        public override string ToString() =>
            string.IsNullOrWhiteSpace(Link)
            ? Title
            : $"{Title}: {Link}";
    }
}
