module FsReveal.FsRevealIntroTest

open FsReveal
open Xunit

[<Fact>]
let ``can read FsReveal intro``() = 
    let doc = FsReveal.GenerateOutputFromMarkdownFile("Index.md", "." ,"index.html")

    Assert.True (System.IO.File.Exists "index.html")


[<Fact>]
let ``can create intro twice``() = 
    let doc = FsReveal.GenerateOutputFromMarkdownFile("Index.md", ".", "index.html") 
    let doc = FsReveal.GenerateOutputFromMarkdownFile("Index.md", ".", "sample.html")

    Assert.True (System.IO.File.Exists "index.html")
    Assert.True (System.IO.File.Exists "sample.html")