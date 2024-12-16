using System;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SpotifyToYouTube;
using SpotifyToYouTube.MusicClients;
using SpotifyToYouTube.MusicClients.Abstractions;

public class Program
{
    private const string SpotifyClientId = "";
    private const string SpotifyClientSecret = "";
    private const string YouTubeApiKey = "";

    public static async Task Main(string[] args)
    {
        try
        {
            var musicClients = Enum.GetValues<MusicClient>();

            var source = SelectEnumOption("Convert from", musicClients);

            Console.WriteLine();

            var destination = SelectEnumOption("Convert to", musicClients.Where(m => m != source).ToArray());

            Console.WriteLine();

            var sourcePlaylistLink = GetInput($"Enter a {source} playlist URL:");

            var sourceMuiscClient = await CreateMusicClientAsync(source);
            var destinationMusicClient = await CreateMusicClientAsync(destination);

            var musicConverter = new MusicConverter(sourceMuiscClient, destinationMusicClient);

            var results = await musicConverter.ConvertAsync(sourcePlaylistLink);

            Console.WriteLine();

            foreach (var track in results)
            {
                Console.WriteLine(track);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    private static async Task<IMusicClient> CreateMusicClientAsync(MusicClient musicClient) =>
        musicClient switch
        {
            MusicClient.Spotify => await Spotify.CreateAuthorizedClientAsync(SpotifyClientId, SpotifyClientSecret),
            MusicClient.YouTube => YouTube.CreateAuthorizedClient(YouTubeApiKey)
        };

    private static T SelectEnumOption<T>(string prompt, params T[] options)
        where T : struct, Enum
    {
        Console.WriteLine(prompt);
        Console.WriteLine(
        $"""
        {string.Join("\n", options.Select((option, i) => $"{i}) {option}"))}
        """);
        Console.Write("Input: ");

        T? selectedOption = null;
        ConsoleKeyInfo? selection = null;
        while (!selectedOption.HasValue || (selection == null || selection!.Value.Key != ConsoleKey.Enter))
        {
            selection = Console.ReadKey(true);

            var isInteger = int.TryParse(selection.Value.KeyChar.ToString(), out var number);

            if (isInteger && number >= 0 && number <= options.Length - 1)
            {
                Console.Write(number);
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);

                selectedOption = options[number];
            }
        }

        Console.WriteLine();

        return selectedOption!.Value;
    }

    private static string GetInput(string prompt)
    {
        Console.WriteLine(prompt);
        var input = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(input));
       
        return input;
    }
}
