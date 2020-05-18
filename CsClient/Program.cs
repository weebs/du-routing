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
            // Console.WriteLine("Hello World!");
            // var c = typeof(SongServer.Api.ReverseString);
            // var d = typeof(SongServer.Api);
            // var z = new MyDelegate<string, string>(SongServer.Api.NewReverseString);
            // Func<DuRouter.ServerRequest<string, string>, SongServer.Api> f = SongServer.Api.NewReverseString;
            // SongServer.Api.
            // DuRouter.makeCsFetch(c,
            //     DuRouter.ServerRequest<string, string>.Create("foo"));
            // var x = SongServer.Api.NewReverseString(DuRouter.ServerRequest<string,string>.Create("foo"));
            // var x = DuRouter.makeCs<string, string, SongServer.Api>(SongServer.Api.NewReverseString, "foo");
            // var x = DuRouter.makeCs<SongServer.Api>();
            // var x = DuRouter.fooz<string, string>();
            // var y = x("foo");
            // m.Oof(SongServer.Api.NewReverseString, "foo");
            
            // DuRouter.doots<string,string,SongServer.Api>(SongServer.Api.NewReverseString, "foo");
            
            // var bar = m.Request<string, string>("foo", SongServer.Api.NewReverseString);
            
            var api = new DuRouter.Api<Api>(SongServer.router);
            
            api.Send(NewFindSongById, 1, out FSharpOption<Song> song);
            var otherSong = api.Request<int, FSharpOption<Song>>(NewFindSongById, 2);
            Song[] kilterSongs = api.Request<string, Song[]>(NewFindSongsByName, "kilter");
            Console.WriteLine("Done");
        }
    }
}