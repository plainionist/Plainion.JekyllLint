namespace Plainion.JekyllLint.Specs

open NUnit.Framework
open FsUnit
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases

[<TestFixture>]
module ``Given a rule implementation`` =

    [<Test>]
    let ``<When> rule is without configuration <Then> rule id can be extracted``() =
        <@ Rules.PageTitleMissing @> 
        |> compileRule
        |> fst
        |> should equal (RuleId("JL0001"))

    [<Test>]
    let ``<When> rule is configured <Then> rule id can be extracted``() =
        <@ Rules.PageTitleTooLong 60 @> 
        |> compileRule
        |> fst
        |> should equal (RuleId("JL0002"))

