// Learn more about F# at http://fsharp.org
module DuRouter

open System
open System.Text
open Newtonsoft.Json

type Response internal (ret: obj, t: Type) =
    override this.ToString() =
        JsonConvert.SerializeObject(ret, t, null)
    
type ServerRequest<'req, 'res> = ServerRequest of 'req * 'res
//    {
//        Request: 'req
//        Response: 'res
//    }
    with
    member this.HandleWith<'a>(f: 'req -> 'res) : Response =
        match this with
        | ServerRequest (req, response) ->
            Response(f req, typeof<'res>)
//            response.Return <- f req
        
let makeRequest (duCase: ServerRequest<'req, 'ret> -> 'du) (req: 'req) =
    let request = duCase (ServerRequest (req, Unchecked.defaultof<'ret>))
    JsonConvert.SerializeObject(request, typeof<'req>, null)
    
let inline routeDu path (f: 'req -> Response) =
    f (Unchecked.defaultof<'req>)
    
let makeFetch router =
    fun (duCase: ServerRequest<'req, 'ret> -> 'du) (req: 'req) ->
        let requestBody = makeRequest duCase req
        let response = router (JsonConvert.DeserializeObject<'du>(requestBody))
        let responseBody = response.ToString()
        let returnVal = JsonConvert.DeserializeObject<'ret>(responseBody.ToString())
        returnVal
    
