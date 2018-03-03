namespace Plainion.JekyllLint.Specs

open NUnit.Framework
open FsUnit
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases

[<TestFixture>]
module ``Given a page with invalid title`` =
    open System.Net.Sockets

    [<Test>]
    let ``<When> title is missing <Then> JL0001 reports a finding``() =
        [
            "some content"
        ]
        |> page
        |> validate <@ Rules.TitleMissing @> 
        |> finding
        |> should equal ("JL0001","Page has no title")

    [<Test>]
    let ``<When> title is missing <and> 'lint-nowarn: JL0001' is defined <Then> JL0001 does not report any finding``() =
        [
            "---"
            "lint-nowarn: JL0001"
            "---"
            "some content"
        ]
        |> page
        |> validate <@ Rules.TitleMissing @> 
        |> should be Empty

    [<Test>]
    let ``<When> title is too long <Then> rule JL0002 reports a finding``() =
        [
            "---"
            "title: this title is longer than it should be and so the rule will complain about the length"
            "---"
            "some content"
        ]
        |> page
        |> validate <@ Rules.TitleTooLong @> 
        |> finding
        |> should equal ("JL0002","Title too long: 85 chars > 60 chars")

    [<Test>]
    let ``<When> title is too long <and> 'lint-nowarn: JL0002' is defined <Then> JL0002 does not report any finding``() =
        [
            "---"
            "title: this title is longer than it should be and so the rule will complain about the length"
            "lint-nowarn: JL0002"
            "---"
            "some content"
        ]
        |> page
        |> validate <@ Rules.TitleTooLong @> 
        |> should be Empty

[<TestFixture>]
module ``Given a page with non-optimal description`` =
    [<Test>]
    let ``<When> description is missing <Then> rule JL0004 complains``() =
        [
            "---"
            "title: hhh"
            "---"
            "some content"
        ]
        |> page
        |> validate <@ Rules.DescriptionMissing @> 
        |> finding
        |> should equal ("JL0004","Page has no description")

    [<Test>]
    let ``<When> description is too long <Then> rule JL0005 complains``() =
        [
            "---"
            "description: this title is longer than it should be. it is very looooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooog"
            "---"
            "some content"
        ]
        |> page
        |> validate <@ Rules.DescriptionLengthNotOptimal @> 
        |> finding
        |> should equal ("JL0005","Description too long: 372 chars > 300 chars")

    [<Test>]
    let ``<When> description is multi-line and too long <Then> rule JL0005 complains``() =
        [
            "---"
            "description: | "
            "  this title is longer than it should be. "
            "  it is very looooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooog"
            "---"
            "some content"
        ]
        |> page
        |> validate <@ Rules.DescriptionLengthNotOptimal @> 
        |> finding
        |> should equal ("JL0005","Description too long: 371 chars > 300 chars")


    [<Test>]
    let ``<When> description is too short <Then> rule JL0005 complains``() =
        [
            "---"
            "description: far too short"
            "---"
            "some content"
        ]
        |> page
        |> validate <@ Rules.DescriptionLengthNotOptimal @> 
        |> finding
        |> should equal ("JL0005","Description too short: 13 chars < 50 chars")
