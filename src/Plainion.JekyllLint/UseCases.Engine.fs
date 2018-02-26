module Plainion.JekyllLint.UseCases.Engine

open Plainion.JekyllLint.Entities

let validatePage rules page =
    let finding id severity msg =
        { Id = id
          Page = page
          Severity = severity
          Message = msg }
    
    rules
    |> Seq.filter(fun ((id,_),_) -> page.Header.NoWarn |> Seq.contains id |> not)
    |> Seq.choose(fun ((id,rule),severity) -> page |> rule |> Option.map (finding id severity))

let reportFinding finding =
    printfn "%s: %s %s: %s" finding.Page.Location (finding.Severity.ToString()) finding.Id.Value finding.Message
