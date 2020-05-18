// Learn more about F# at http://fsharp.org

module SongServer

open DuRouter
open System.Text
    
    
// Models
type SongType =
    | Original
    | Feature of artists: string list
    | Remix of original: Song
    | VIP of original: Song
    | Cover of original: Song
and Song = { Id: int; Name: string; SongType: SongType; Artists: string list }
    
// API definition
type Api =
    | FindSongById of ServerRequest<int, Song option>
    | FindSongsByName of ServerRequest<string, Song[]>
    | ReverseString of ServerRequest<string, string>
    
let findSongById (id: int) : Song option =
    let songDb = Map.ofList [
        1, { Id = 1; Name = "Funk Blaster"; Artists = [ "KOAN Sound" ]; SongType = Original }
        2, { Id = 2; Name = "Cozza Frenzy"; Artists = [ "Bassnectar" ]; SongType = Original }
        3, { Id = 3; Name = "Off Kilter"; Artists = [ "Tipper" ]; SongType = Original }
        4, { Id = 4; Name = "Off Kilter"; Artists = [ "Tipper" ]; SongType = VIP {
                Id = 3; Name = "Off Kilter"; Artists = [ "Tipper" ]; SongType = Original } }
    ]
    if songDb.ContainsKey(id) then
        Some songDb.[id]
    else
        None
        
let findSongsByName (name: string) : Song[] =
    let songDb = Map.ofList [
        1, { Id = 1; Name = "Funk Blaster"; Artists = [ "KOAN Sound" ]; SongType = Original }
        2, { Id = 2; Name = "Cozza Frenzy"; Artists = [ "Bassnectar" ]; SongType = Original }
        3, { Id = 3; Name = "Off Kilter"; Artists = [ "Tipper" ]; SongType = Original }
        4, { Id = 4; Name = "Off Kilter"; Artists = [ "Tipper" ]; SongType = VIP {
                Id = 3; Name = "Off Kilter"; Artists = [ "Tipper" ]; SongType = Original } }
    ]
    songDb
    |> Map.filter (fun id song -> song.Name.ToLower().Contains(name.ToLower()))
    |> Map.toArray
    |> Array.map snd
    
let router (request: Api) : Response =
    match request with
    | FindSongById r -> r.HandleWith(findSongById)
    | FindSongsByName r -> r.HandleWith(findSongsByName)
    | ReverseString r -> r.HandleWith(fun s ->
        let reversed = StringBuilder()
        for index in [ (s.Length - 1) .. -1 .. 0 ] do
            reversed.Append(s.[index]) |> ignore
        reversed.ToString()
    )
