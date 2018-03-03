namespace Plainion.JekyllLint.Specs

open NUnit.Framework
open FsUnit
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases

[<TestFixture>]
module ``Given a page with short content`` =
    [<Test>]
    let ``<When> content is too short <Then> rule JL0003 complains``() =
        [
            "this is just some dummy content shorter than expected"
        ]
        |> page
        |> validate <@ Rules.ContentTooShort 10 @> 
        |> finding
        |> should equal ("JL0003","Content too short: 9 words < 10 words")

[<TestFixture>]
module ``Given a link to and image`` =
    [<Test>]
    let ``<When> image link is with markdown syntax <and> 'alt' text is missing <Then> rule JL0006 complains``() =
        [
            "![alt text](http://www.123.net/my.png)"
            "![](http://www.123.net/my.png)"
        ]
        |> page
        |> validate <@ Rules.ImageHasNoAltText @> 
        |> finding
        |> should equal ("JL0006","Image in line 2 has no alt text")

    [<Test>]
    let ``<When> image link is with HTML syntax <and> 'alt' text is missing <Then> rule JL0006 complains``() =
        [
            "<img src=\"http://www.123.net/my.png\" alt=\"alt text\"/>"
            "<img src=\"http://www.123.net/my.png\"/>"
        ]
        |> page
        |> validate <@ Rules.ImageHasNoAltText @> 
        |> finding
        |> should equal ("JL0006","Image in line 2 has no alt text")

    [<Test>]
    let ``<When> image link is with markdown syntax <and> 'title' text is missing <Then> rule JL0007 complains``() =
        [
            "![](http://www.123.net/my.png \"title text\")"
            "![](http://www.123.net/my.png)"
        ]
        |> page
        |> validate <@ Rules.ImageHasNoTitleText @> 
        |> finding
        |> should equal ("JL0006","Image in line 2 has no title text")

    [<Test>]
    let ``<When> image link is with HTML syntax <and> 'title' text is missing <Then> rule JL0007 complains``() =
        [
            "<img src=\"http://www.123.net/my.png\" title=\"title text\"/>"
            "<img src=\"http://www.123.net/my.png\"/>"
        ]
        |> page
        |> validate <@ Rules.ImageHasNoTitleText @> 
        |> finding
        |> should equal ("JL0006","Image in line 2 has no title text")

