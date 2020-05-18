open SongServer

[<EntryPoint>]
let main argv =
    let fetch = DuRouter.makeFetch SongServer.router
    let songs = fetch Api.FindSongsByName "kilter"
    
    printfn "Found %d songs" songs.Length
    for song in songs do
        printfn "%A" song
    0 // return an integer exit code
