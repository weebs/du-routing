// Learn more about F# at http://fsharp.org

open System
open Newtonsoft
open Newtonsoft.Json

type Response<'ret> =
    {
        mutable Return: 'ret
    }

//type ServerResponse<'ret> = ServerResponse of 'ret

type Dooter internal (ret: obj, t: Type) =
    override this.ToString() =
        JsonConvert.SerializeObject(ret, t, null)
    
type ServerRequest<'req, 'res> = ServerRequest of 'req * 'res
//    {
//        Request: 'req
//        Response: 'res
//    }
    with
    member this.HandleWith<'a>(f: 'req -> 'res) : Dooter =
        match this with
        | ServerRequest (req, response) ->
            Dooter(f req, typeof<'ret>)
//            response.Return <- f req

//let inline handleRequest (request: ServerRequest<'req, 'ret>) (f: 'req -> 'ret) : ServerResponse<'ret> =
//let inline handleRequest (request: ServerRequest<'req, 'ret>) (f: 'req -> 'ret) : unit =
//    match request with
//    | ServerRequest (req, response) ->
//            response.Return <- ()

type Complex = { Id: int; Name: string }
    
type Cuties =
    | Fuzzies of Fuzzy
    | Doggie
    | Kittie
and Fuzzy =
    | Cowsie
    | Bunnie
    | Foxie
    
//type Foo =
//    | GetBar of ServerRequest<
type Request =
    | GetName of ServerRequest<int, string>
    | GetId of ServerRequest<string, int>
    | FindCowsie of ServerRequest<unit, Complex>
    | FindCutie of ServerRequest<unit, Cuties>
    
//let inline handleRequest (request: ServerRequest<'req, 'ret>) (f: 'req -> 'ret) =
//    match request with
//    | ServerRequest (req, response) ->
//        response.Return <- f req
        
let inline makeRequest (duCase: ServerRequest<'req, 'ret> -> 'du) (req: 'req) = duCase (ServerRequest (req, Unchecked.defaultof<'ret>))
    
let getName id =
    match id with
    | 1 -> "Dave"
    | _ -> "Unknown"
    
let getId name =
    match name with
    | "Dave" -> 1
    | _ -> 0
    
let findCowsie () =
    { Id = 1337; Name = "cowsay moo" }
    
let findCutie () =
    Cuties.Fuzzies (Fuzzy.Cowsie)
    
let router (request: Request) : Dooter =
    match request with
    | GetName r -> r.HandleWith(getName) //handleRequest r getName
    | GetId r -> r.HandleWith(getId) //r.HandleWith(getId)
    | FindCowsie r -> r.HandleWith(findCowsie)
    | FindCutie r -> r.HandleWith(findCutie)
    
let inline routeDu path (f: 'req -> Dooter) =
    f (Unchecked.defaultof<'req>)
    
routeDu "/api" router
    
[<EntryPoint>]
let main argv =
    let req = makeRequest Request.FindCowsie () 
    let ret = router req
    let req' = makeRequest Request.FindCutie () 
    let ret' = router req'
    printfn "Hello World from F#!"
    printfn "%A" ret
    printfn "%A" ret'
    0 // return an integer exit code
