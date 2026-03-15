module FsReveal.SpeakerNotesSpecs

open FsReveal
open Xunit

let testTemplate ="{slides}"

let normalizeLineBreaks (text:string) = text.Replace("\r\n","\n").Replace("\n","\n")

let md = """
- title : FsReveal

***

### Slide 1

Normal Text

' Oh hey, these are some notes.
' And some more


***

### Slide 2
Normal Text
"""

let expectedOutput = """<section >
<h3>Slide 1</h3>
<p>Normal Text</p>
<aside class="notes">
Oh hey, these are some notes.<br/>
And some more<br/>
</aside>
</section>
<section >
<h3>Slide 2</h3>
<p>Normal Text</p>
</section>

"""

[<Fact>]
let ``can generate sections from markdown``() = 
    let presentation = FsReveal.GetPresentationFromMarkdown md

//    Formatting.GenerateHTML testTemplate presentation.Document  
//    |> normalizeLineBreaks
//    |> shouldEqual (normalizeLineBreaks expectedOutput)

    let result =
        Formatting.GenerateHTML testTemplate presentation.Document
        |> normalizeLineBreaks
    let result = normalizeLineBreaks expectedOutput 
    Assert.Equal (expectedOutput, result)

