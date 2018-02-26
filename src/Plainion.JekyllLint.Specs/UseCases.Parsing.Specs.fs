namespace Plainion.JekyllLint.Specs

open NUnit.Framework
open FsUnit
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases

[<TestFixture>]
module ``Given raw lines of a page front matter`` =
    [<Test>]
    let ``<When> contains single line 'title' attribute <Then> title value is extracted``() =
        [
            "---"
            "title: some title"
            "---"
        ]
        |> Parsing.getHeader
        |> (fun h -> h.Title)
        |> should equal (Some "some title")

    [<Test>]
    let ``<When> contains multi line 'description' <Then> description full value is extracted as one concatenated string``() =
        [
            "---"
            "description: | "
            "  first line."
            "  second line"
            "---"
        ]
        |> Parsing.getHeader
        |> (fun h -> h.Attributes |> Map.find("description"))
        |> should equal "first line.second line"

