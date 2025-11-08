open System.Diagnostics
open System.IO
open FSharp.Formatting.Literate.Evaluation
open Fake.Core
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

open FsReveal
open Suave
open Suave.Operators
open Suave.Sockets.Control
open Suave.WebSocket
open Suave.Utils
open Suave.Files

let operator (</>) a b =  Path.Combine (a, b)
let outDir = __SOURCE_DIRECTORY__ </> "output"
let slidesDir = __SOURCE_DIRECTORY__ </> "slides"

// https://github.com/TheAngryByrd/MiniScaffold

(*let fsiEvaluator = 
    let evaluator = FSharp.Literate.FsiEvaluator()
    evaluator.EvaluationFailed.Add(fun err -> 
        Trace.traceImportant <| sprintf "Evaluating F# snippet failed:\n%s\nThe snippet evaluated:\n%s" err.StdErr err.Text )
    evaluator 
*)
let copyStylesheet() =
    try
        Shell.copyFile (outDir </> "css" </> "custom.css") (slidesDir </> "custom.css")
    with
    | exn -> Trace.traceImportant <| sprintf "Could not copy stylesheet: %s" exn.Message

let copyPics() =
    try
      Shell.copyDir (outDir </> "images") (slidesDir </> "images") (fun f -> true)
    with
    | exn -> Trace.traceImportant <| sprintf "Could not copy picture: %s" exn.Message


let generateFor (file:FileInfo) = 
    try
        Trace.traceImportant <| sprintf "Generating slides for: %s" file.FullName
        copyPics()
        let rec tryGenerate trials =
            try
                FsReveal.GenerateFromFile(file.FullName, outDir) //, fsiEvaluator = (fsiEvaluator :> IFsiEvaluator)) //#Todo
            with 
            | exn when trials > 0 -> tryGenerate (trials - 1)
            | exn -> 
                Trace.traceImportant <| sprintf "Could not generate slides for: %s" file.FullName
                Trace.traceImportant exn.Message

        tryGenerate 3

        copyStylesheet()
    with
    | :? FileNotFoundException as exn ->
        Trace.traceImportant <| sprintf "Could not copy file: %s" exn.FileName

let refreshEvent = new Event<_>()

let handleWatcherEvents (events:FileChange seq) =
    for e in events do
        let fi = new FileInfo( e.FullPath )
        Trace.traceImportant <| sprintf "%s was changed." fi.Name
        match fi.Attributes.HasFlag FileAttributes.Hidden || fi.Attributes.HasFlag FileAttributes.Directory with
        | true -> ()
        | _ -> generateFor fi
//        | _ -> printfn $"%s{fi.FullName}"
    refreshEvent.Trigger()

let socketHandler (webSocket : WebSocket.WebSocket) =
  fun cx -> socket {
    while true do
      let! refreshed =
        Control.Async.AwaitEvent(refreshEvent.Publish)
        |> Suave.Sockets.SocketOp.ofAsync 
      do! webSocket.send Text (ASCII.bytes "refreshed") true
  }

let startWebServer () =
    let rec findPort port =
        let portIsTaken =
            System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
            |> Seq.exists (fun x -> x.Port = port)

        if portIsTaken then findPort (port + 1) else port

    let port = findPort 8083

    let di = new DirectoryInfo(outDir)
    let serverConfig = 
        { defaultConfig with
           homeFolder = Some di.FullName
           bindings = [ HttpBinding.createSimple HTTP "127.0.0.1" port ]
        }
    let app =
      choose [
        Filters.path "/websocket" >=> WebSocket.handShake( socketHandler )
        Writers.setHeader "Cache-Control" "no-cache, no-store, must-revalidate"
        >=> Writers.setHeader "Pragma" "no-cache"
        >=> Writers.setHeader "Expires" "0"
        >=> browseHome ]
    startWebServerAsync serverConfig app |> snd |> Async.Start
    
    Directory.SetCurrentDirectory(outDir)
    
    
    //let startInfo = new ProcessStartInfo( sprintf "http://localhost:%d/index.html" port )
    let startInfo = new ProcessStartInfo( "firefox")
    startInfo.Arguments <- sprintf "http://localhost:%d/index.html" port
    startInfo.UseShellExecute <- false
    startInfo.WorkingDirectory <- outDir
    use proc = Process.Start(startInfo)
    proc.WaitForExit()
    //System.Diagnostics.Process.Start (sprintf "http://localhost:%d/index.html" port) |> ignore
    //Shell.Exec (sprintf "http://localhost:%d/index.html" port) |> ignore


//let initTargets () =
//    Target.create "GenerateSlides" (fun _ ->
let generateSlides =         
    let fileInfo fn = new FileInfo(fn)
    !! (slidesDir + "/**/*.md")
      ++ (slidesDir + "/**/*.fsx")
    |> Seq.map fileInfo
    |> Seq.iter generateFor
//    )
    
//    Target.create "KeepRunning" (fun _ ->
let keepRunning =
    use watcher = !! (slidesDir + "/**/*.*") |> ChangeWatcher.run handleWatcherEvents
        
    startWebServer() 

    Trace.traceImportant "Waiting for slide edits. Press any key to stop."

    System.Console.ReadKey() |> ignore

    watcher.Dispose()
//    )

(*
    Target.create "Default" (fun _ ->
        printfn "Done"
    )

    Target.create "Clean" (fun _ ->
        Shell.cleanDirs [outDir]
    )

    "Clean"
      ==> "GenerateSlides"
      ==> "KeepRunning"
      ==> "Default"
    |> ignore
*)


[<EntryPoint>]
let main argv =

    Trace.traceImportant  "Starting up..."
    Trace.traceImportant  <| sprintf "outdir is set to %s" outDir
    Trace.traceImportant  <| sprintf "slides is set to %s" slidesDir
    
    generateSlides
    keepRunning
    (* argv
    |> Array.toList
    |> Context.FakeExecutionContext.Create false "build.fsx"
    |> Context.RuntimeContext.Fake
    |> Context.setExecutionContext

    initTargets ()
    Target.runOrDefaultWithArguments ("KeepRunning")*)
    
    Trace.traceImportant  "Done"
    0 // return an integer exit code
    