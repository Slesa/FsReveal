namespace FsReveal

open System
open System.IO
open System.Collections.Generic
open System.Text
open FSharp.Formatting.Literate
open FSharp.Formatting.Markdown
//open FSharp.Formatting.Html

type SlideData = 
    { Properties : Map<string,string>
      Paragraphs : MarkdownParagraph list }

type Slide = 
    | Simple of SlideData
    | Nested of SlideData list

type Presentation = 
    { Properties : Map<string,string>
      Slides : Slide list
      Document : LiterateDocument }
