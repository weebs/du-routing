module DuRouter

open System
open Newtonsoft.Json

type Response internal (ret: obj, t: Type) =
    override this.ToString() =
        JsonConvert.SerializeObject(ret, t, null)
    
type ServerRequest<'req, 'res>(req: 'req) =
    member this.Request = req
    member this.Response = Unchecked.defaultof<'res>
    with
    // todo: An override for HandleWith could be created that makes it simple to write handlers in C#
    member this.HandleWith(f: 'req -> 'res) : Response =
        Response(f this.Request, typeof<'res>)
        
let makeRequestBody (duCase: ServerRequest<'req, 'ret> -> 'du) (req: 'req) =
    let request = duCase (ServerRequest(req))
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
        let request = makeDuCase.Invoke(ServerRequest(req))
        let requestBody = JsonConvert.SerializeObject(request, typeof<'du>, null)
        let response = router.Invoke(JsonConvert.DeserializeObject<'du>(requestBody))
        let responseBody = response.ToString()
        let returnVal = JsonConvert.DeserializeObject<'res>(responseBody.ToString())
        res <- returnVal
        
    member this.Request<'req, 'res>(makeDuCase: Func<ServerRequest<'req, 'res>, 'du>, req: 'req) : 'res =
        let request = makeDuCase.Invoke(ServerRequest(req))
        let requestBody = JsonConvert.SerializeObject(request, typeof<'du>, null)
        let response = router.Invoke(JsonConvert.DeserializeObject<'du>(requestBody))
        let responseBody = response.ToString()
        let returnVal = JsonConvert.DeserializeObject<'res>(responseBody.ToString())
        returnVal
        
    // todo: can this work while being type safe?
    member this.Fetch<'res>(req: 'du) : 'res =
        let res = router.Invoke(req)
        JsonConvert.DeserializeObject<'res>(res.ToString())
