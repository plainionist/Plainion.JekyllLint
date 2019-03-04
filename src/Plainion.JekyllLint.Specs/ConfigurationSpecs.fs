namespace Plainion.JekyllLint.Specs

open NUnit.Framework
open FsUnit
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases

[<TestFixture>]
module ``Given all known rules`` =
    [<Test>]
    let ``<When> compiled <Then> rule ids are uniq``() =
        Rules.All
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

[<TestFixture>]
module ``Given a severity mapping`` =
    [<Test>]
    let ``<When> no special interpretation is configured <and> finding severity is warning <Then> finding is reported as warning``() =
        Warning
        |> Engine.mapSeverity Engine.AsIs
        |> should equal Warning
        
    [<Test>]
    let ``<When> no special interpretation is configured <and> finding severity is error <Then> finding is reported as error``() =
        Error
        |> Engine.mapSeverity Engine.AsIs
        |> should equal Error  
    
    [<Test>]
    let ``<When> 'warning to error' is configured <and> finding severity is warning <Then> finding is reported as error``() =
        Warning
        |> Engine.mapSeverity Engine.WarningToError
        |> should equal Error     

    [<Test>]
    let ``<When> 'error to warning' is configured <and> finding severity is error <Then> finding is reported as warning``() =
        Error
        |> Engine.mapSeverity Engine.ErrorToWarning
        |> should equal Warning 