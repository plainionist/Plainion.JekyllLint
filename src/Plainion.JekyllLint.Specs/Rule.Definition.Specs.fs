namespace Plainion.JekyllLint.Specs

open NUnit.Framework
open FsUnit
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases

[<TestFixture>]
module ``Given all known rules`` =
    open Plainion.JekyllLint

    [<Test>]
    let ``<When> compiled <Then> rule ids are uniq``() =
        Main.rules
        |> List.map(fun ((id,_),_) -> id)
        |> should equal [ 
                            RuleId("JL0001")
                            RuleId("JL0002")
                            RuleId("JL0003")
                            RuleId("JL0004")
                            RuleId("JL0005")
                            RuleId("JL0006")
                            RuleId("JL0007")
                        ]

