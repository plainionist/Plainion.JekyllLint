namespace Plainion.JekyllLint.Specs

open NUnit.Framework
open FsUnit

[<TestFixture>]
module ``Given a file with front matter`` =
    open Plainion.JekyllLint.UseCases.Parsing

    [<Test>]
    let ``<When> contains single line key-value pair <Then> key and value are extracted``() =
        [
            "---"
            "title: some title"
            "---"
        ]
        |> GetHeader
        |> (fun h -> h.Title)
        |> should equal (Some "some title")

    [<Test>]
    let ``<When> contains multi line key-value pair <Then> key and multi line value are extracted``() =
        [
            "---"
            "description: | "
            "  first line."
            "  second line"
            "---"
        ]
        |> GetHeader
        |> (fun h -> h.Attributes |> Map.find("description"))
        |> should equal "first line.second line"

