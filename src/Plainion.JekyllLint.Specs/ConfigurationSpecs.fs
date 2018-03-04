namespace Plainion.JekyllLint.Specs

open NUnit.Framework
open FsUnit
open Plainion.JekyllLint
open Plainion.JekyllLint.Entities

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

[<TestFixture>]
module ``Given a severity interpretation`` =
    open Plainion.JekyllLint.UseCases

    let finding severity = 
        { Id = 1 |> createRuleId
          Page = [] |> page
          LineNumber = 1
          Severity = severity
          Message = "" }
        
    [<Test>]
    let ``<When> no special interpretation is configured <and> finding severity is warning <Then> finding is reported as warning``() =
        finding Warning
        |> Engine.reportFinding AsIs
        |> should equal Warning
        
    [<Test>]
    let ``<When> no special interpretation is configured <and> finding severity is error <Then> finding is reported as error``() =
        finding Error
        |> Engine.reportFinding AsIs
        |> should equal Error  
    
    [<Test>]
    let ``<When> 'warning to error' is configured <and> finding severity is warning <Then> finding is reported as error``() =
        finding Warning
        |> Engine.reportFinding WarningToError
        |> should equal Error     

    [<Test>]
    let ``<When> 'error to warning' is configured <and> finding severity is error <Then> finding is reported as warning``() =
        finding Error
        |> Engine.reportFinding ErrorToWarning
        |> should equal Warning 