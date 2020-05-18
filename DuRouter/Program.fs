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
    static member Create(req: 'req) = ServerRequest (req, Unchecked.defaultof<'res>)
    member this.HandleWith<'a>(f: 'req -> 'res) : Response =
        match this with
        | ServerRequest (req, response) ->
            Response(f req, typeof<'res>)
//            response.Return <- f req
        
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
        
//    member this.Request2(req: 'req, makeDuCase: Func<ServerRequest<_, _>, 'du>) : 'res =
//        makeDuCase.Invoke(ServerRequest (req, Unchecked.defaultof<'res>))
//        Unchecked.defaultof<'res>
//    member inline this.Request<'res, 'req>(req: 'req, makeDuCase: Func<ServerRequest<'req, 'res>, 'du>) : 'res =
//        makeDuCase.Invoke(ServerRequest (req, Unchecked.defaultof<'res>))
//        Unchecked.defaultof<'res>
//let makeDoots<'du> () =
//    fun makeDuCase -> doots<'req, 'res, 'du> makeDuCase
//    Func<Func<ServerRequest<'req, 'res>, 'du>, 'du>(fun req)
let inline makeCsFetch (typ: Type) f =
//    let s = new ^t()
//    let s = (^t : (member new : string -> ^t) (t, "s"))
//      let typ = typeof< ^t >
      let cases = Microsoft.FSharp.Reflection.FSharpType.GetUnionCases typ
      //Microsoft.FSharp.Reflection.FSharpValue.MakeUnion
      let constructors = typ.GetConstructors(Reflection.BindingFlags.FlattenHierarchy)
      let fields = typ.GetFields(Reflection.BindingFlags.NonPublic)
      let members = typ.GetMembers(Reflection.BindingFlags.NonPublic)
      ()
//let inline makeCsFetch2 (t: ^t) (f: 'req) =
//      (^t : (member set_Item : ServerRequest<'req, 'res> -> unit) (t, (ServerRequest (f, Unchecked.defaultof<'res>)))
//      t
//    printfn "%A" t
//    s
//    fun (duCase: 'du) (req: 'req) ->
//        let requestBody = JsonConvert.SerializeObject(duCase, typeof<'req>, null)
//        let response = router (JsonConvert.DeserializeObject<'du>(requestBody))
//        let responseBody = response.ToString()
//        let returnVal = JsonConvert.DeserializeObject<'ret>(responseBody.ToString())
//        returnVal
    
