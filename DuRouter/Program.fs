// Learn more about F# at http://fsharp.org

open System
open Newtonsoft
open Newtonsoft.Json

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
            Dooter(f req, typeof<'res>)
//            response.Return <- f req

type Complex = { Id: int; Name: string }
    
type Cuties =
    | Fuzzies of Fuzzy
    | Doggie
    | Kittie
and Fuzzy =
    | Cowsie
    | Bunnie
    | Foxie
    
type Request =
    | GetName of ServerRequest<int, string>
    | GetId of ServerRequest<string, int>
    | FindCowsie of ServerRequest<unit, Complex>
    | FindCutie of ServerRequest<unit, Cuties>
        
let makeRequest (duCase: ServerRequest<'req, 'ret> -> 'du) (req: 'req) =
    let request = duCase (ServerRequest (req, Unchecked.defaultof<'ret>))
    JsonConvert.SerializeObject(request, typeof<'req>, null)
    
    
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
    
let router (request: string) : Dooter =
    let request = JsonConvert.DeserializeObject<Request>(request)
    match request with
    | GetName r -> r.HandleWith(getName) //handleRequest r getName
    | GetId r -> r.HandleWith(getId) //r.HandleWith(getId)
    | FindCowsie r -> r.HandleWith(findCowsie)
    | FindCutie r -> r.HandleWith(findCutie)
    
let inline routeDu path (f: 'req -> Dooter) =
    f (Unchecked.defaultof<'req>)
    
let fetch (duCase: ServerRequest<'req, 'ret> -> 'du) (req: 'req) =
    let requestBody = makeRequest duCase req
    let response = router requestBody
    let responseBody = response.ToString()
    let returnVal = JsonConvert.DeserializeObject<'ret>(responseBody.ToString())
    returnVal
    
[<EntryPoint>]
let main argv =
    let cowsie = fetch Request.FindCowsie ()
    printfn "%A" cowsie
    0 // return an integer exit code
