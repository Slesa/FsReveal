
#I @"packages/FsReveal/fsreveal/"
//#I @"packages/FAKE/tools/"
#I @"packages/Suave/lib/netstandard2.1"

#r "paket:
nuget Fake.Core.Target //"


//#r "FakeLib.dll"
#r "Suave.dll"
open Fake.Core

#load "fsreveal.fsx"

// Git configuration (used for publishing documentation in gh-pages branch)
// The profile where the project is posted
let gitOwner = "myGitUser"
let gitHome = "https://github.com/" + gitOwner
// The name of the project on GitHub
let gitProjectName = "MyProject"
// The name of the GitHub repo subdirectory to publish slides to
let gitSubDir = ""

open FsReveal
//open Fake
//open Fake.Git
open System.IO
// open System.Diagnostics
// open Suave
// open Suave.Web
// open Suave.Http
// open Suave.Operators
// open Suave.Sockets
// open Suave.Sockets.Control
// open Suave.Sockets.AsyncSocket
// open Suave.WebSocket
// open Suave.Utils
// open Suave.Files

(*





Target "ReleaseSlides" (fun _ ->
    if gitOwner = "myGitUser" || gitProjectName = "MyProject" then
        failwith "You need to specify the gitOwner and gitProjectName in build.fsx"
    let tempDocsRoot = __SOURCE_DIRECTORY__ </> "temp/gh-pages"
    let tempDocsDir = tempDocsRoot </> gitSubDir
    CleanDir tempDocsRoot
    Repository.cloneSingleBranch "" (gitHome + "/" + gitProjectName + ".git") "gh-pages" tempDocsRoot

    fullclean tempDocsDir
    CopyRecursive outDir tempDocsDir true |> tracefn "%A"
    StageAll tempDocsRoot
    Git.Commit.Commit tempDocsRoot "Update generated slides"
    Branches.push tempDocsRoot
)
*)


//"GenerateSlides"
//  ==> "ReleaseSlides"
  
//RunTargetOrDefault "KeepRunning"
RunTargetOrDefault "Clean"
