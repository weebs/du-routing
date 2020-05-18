module DuRouter

open System
open Newtonsoft.Json

type Response internal (ret: obj, t: Type) =
    override this.ToString() =
        JsonConvert.SerializeObject(ret, t, null)
    
// todo: Experiment with making ServerRequest a record/struct { Request: 'req; Response: 'res }
type ServerRequest<'req, 'res> = ServerRequest of 'req * 'res
    with
    static member Create(req: 'req) = ServerRequest (req, Unchecked.defaultof<'res>)
    member this.HandleWith<'a>(f: 'req -> 'res) : Response =
        match this with
        | ServerRequest (req, response) ->
            Response(f req, typeof<'res>)
        
let makeRequestBody (duCase: ServerRequest<'req, 'ret> -> 'du) (req: 'req) =
    let request = duCase (ServerRequest (req, Unchecked.defaultof<'ret>))
    JsonConvert.SerializeObject(request, typeof<'req>, null)
    
let inline routeDu path (f: 'req -> Response) =
    f (Unchecked.defaultof<'req>)
    
let makeFetch router =
    fun (duCase: ServerRequest<'req, 'ret> -> 'du) (req: 'req) ->
        let requestBody = makeRequestBody duCase req
        let response = router (JsonConvert.DeserializeObject<'du>(requestBody))
        let responseBody = response.ToString()
        let returnVal = JsonConvert.DeserializeObject<'ret>(responseBody.ToString())
        returnVal
        
type Api<'du>(router: Func<'du, Response>) =
    member this.Send(makeDuCase: Func<ServerRequest<'req, 'res>, 'du>, req: 'req, res: 'res outref) : unit =
        let request = makeDuCase.Invoke(ServerRequest (req, Unchecked.defaultof<'res>))
        let requestBody = JsonConvert.SerializeObject(request, typeof<'du>, null)
        let response = router.Invoke(JsonConvert.DeserializeObject<'du>(requestBody))
        let responseBody = response.ToString()
        let returnVal = JsonConvert.DeserializeObject<'res>(responseBody.ToString())
        res <- returnVal
        
    member this.Request<'req, 'res>(makeDuCase: Func<ServerRequest<'req, 'res>, 'du>, req: 'req) : 'res =
        let request = makeDuCase.Invoke(ServerRequest (req, Unchecked.defaultof<'res>))
        let requestBody = JsonConvert.SerializeObject(request, typeof<'du>, null)
        let response = router.Invoke(JsonConvert.DeserializeObject<'du>(requestBody))
        let responseBody = response.ToString()
        let returnVal = JsonConvert.DeserializeObject<'res>(responseBody.ToString())
        returnVal
