module Plainion.JekyllLint.UseCases.Engine

open Plainion.JekyllLint.Entities

type SeverityMapping =
    | AsIs
    | WarningToError
    | ErrorToWarning

let mapSeverity severityMapping severity =
    match severityMapping,severity with
    | AsIs,x -> x
    | WarningToError,_ -> Error
    | ErrorToWarning,_ -> Warning

let validatePage rules severityMapping page =
    let skipNoWarn ((id,_),_) =
        page.Header.NoWarn |> Seq.contains id |> not

    let applyRule ((id,rule),ruleSeverity) =
        let createFinding (lineNo,msg) =
            { Id = id
              Page = page
              LineNumber = lineNo
              Severity = ruleSeverity |> mapSeverity severityMapping
              Message = msg }
        
        page 
        |> rule 
        |> Seq.map createFinding

    rules
    |> Seq.filter skipNoWarn
    |> Seq.map applyRule
    |> Seq.concat
    |> List.ofSeq

let validatePages getPages rules severityMapping sources =
    sources
    |> getPages
    |> Seq.collect (validatePage rules severityMapping)
    |> List.ofSeq
