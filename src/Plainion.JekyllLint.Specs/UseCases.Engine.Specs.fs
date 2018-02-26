namespace Plainion.JekyllLint.Specs

open NUnit.Framework
open FsUnit
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases

[<TestFixture>]
module ``Given a rule implementation`` =
    [<Test>]
    let ``<When> no configuration is given <Then> rule id can be extracted``() =
        <@ Rules.PageTitleMissing @> 
        |> Engine.compileRule
        |> fst
        |> should equal (RuleId("JL0001"))

    [<Test>]
    let ``<When> configuration is given <Then> rule id can be extracted``() =
        <@ Rules.PageTitleTooLong 60 @> 
        |> Engine.compileRule
        |> fst
        |> should equal (RuleId("JL0002"))

[<TestFixture>]
module ``Given a page to validate`` =
    [<Test>]
    let ``<When> 'lint-nowarn: JL0001,JL0002' is set <Then> these rules are not executed``() =
        let rules = 
            [
                <@ Rules.PageTitleMissing     @> |> Engine.compileRule, Error
                <@ Rules.PageTitleTooLong 60  @> |> Engine.compileRule, Warning
            ]

        ("",[
            "---"
            "lint-nowarn: JL0001,JL0002"
            "---"
        ])
        |> Parsing.createPage
        |> Engine.validatePage rules
        |> should be Empty

