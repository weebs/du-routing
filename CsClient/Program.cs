using System;
using Microsoft.FSharp.Core;
using static SongServer;
using static SongServer.Api;

namespace CsClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new DuRouter.Api<Api>(SongServer.router);
            
            api.Send(NewFindSongById, 1, out FSharpOption<Song> song);
            var otherSong = api.Request<int, FSharpOption<Song>>(NewFindSongById, 2);
            var kilterSongs = api.Request<string, Song[]>(NewFindSongsByName, "kilter");
            Console.WriteLine("Done");
        }
    }
}