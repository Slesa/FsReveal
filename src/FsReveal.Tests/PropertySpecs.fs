module FsReveal.PropertySpecs

open FsReveal
open Xunit

let md = """
- title : FsReveal
- description : Introduction to FsReveal
- author : Karlkim Suwanmongkol
- theme : Night
- transition : default

***

### Section 1

***

### Section 2

---

#### Section 2.1

---

#### Section 2.2

***

### Section 3"""

[<Fact>]
let ``can read properties from markdown``() = 
    let properties = (md |> FsReveal.GetPresentationFromMarkdown).Properties
    Assert.Equal ("FsReveal", properties.["title"])
    Assert.Equal ("Introduction to FsReveal", properties.["description"]) 
    Assert.Equal ("Night", properties.["theme"]) 
    Assert.Equal ("default", properties.["transition"]) 

let defaultMD = """
***

### Section 1
r
***

### Section 2

---

#### Section 2.1

---

#### Section 2.2

***

### Section 3"""

[<Fact>]
let ``uses default properties if nothing is specified in markdown``() = 
    let properties = (defaultMD |> FsReveal.GetPresentationFromMarkdown).Properties
    Assert.Equal ("Presentation", properties.["title"]) 
    Assert.Empty properties.["description"]
    Assert.Equal ("night", properties.["theme"]) 
    Assert.Equal ("default", properties.["transition"]) 