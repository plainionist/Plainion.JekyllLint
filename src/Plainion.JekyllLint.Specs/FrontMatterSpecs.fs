namespace Plainion.JekyllLint.Specs

open NUnit.Framework
open FsUnit
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases

[<TestFixture>]
module ``Given a file with front matter`` =

    [<Test>]
    let ``<When> contains single line key-value pair <Then> key and value are extracted``() =
        [
            "---"
            "title: some title"
            "---"
        ]
        |> Parsing.getHeader
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
        |> Parsing.getHeader
        |> (fun h -> h.Attributes |> Map.find("description"))
        |> should equal "first line.second line"

    [<Test>]
    let ``<When> 'lint-nowarn: JL0001,JL0002' is set <Then> rule JL0001 and JL0002 not executed``() =
        let rules = 
            [
                <@ Rules.PageTitleMissing     @> |> compileRule, Error
                <@ Rules.PageTitleTooLong 60  @> |> compileRule, Warning
            ]

        ("",[
            "---"
            "lint-nowarn: JL0001,JL0002"
            "---"
        ])
        |> Parsing.createPage
        |> Engine.validatePage rules
        |> should be Empty

