#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.Core.Target
nuget Fake.Core.Process
nuget Fake.DotNet.MSBuild
nuget Fake.IO.FileSystem //"
// include Fake modules, see Fake modules section

open Fake.Core
open Fake.DotNet.Cli
open Fake.IO.FileSystem

Target.initEnvironment ()


Target.create "Clean" (fun _ -> !! "src/**/bin" ++ "src/**/obj" |> Shell.cleanDirs)


// *** Define Targets ***
Target.create "Hello" (fun _ ->
  printfn "hello from FAKE!"
)





Target.create "Build" (fun _ -> !! "src/**/*.*proj" |> Seq.iter (DotNet.build id))

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "All"

Target.runOrDefault "All"

