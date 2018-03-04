module Plainion.JekyllLint.Gateways.Controller

open System
open System.IO
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases

let reportFinding finding =
    printfn "%s(%i,0): %s %s: %s" finding.Page.Location finding.LineNumber (finding.Severity.ToString()) (printRuleId finding.Id) finding.Message

let validate getPages validatePage sources =
    sources
    |> getPages
    |> Seq.collect validatePage
    |> Seq.fold(fun severity finding -> 
        finding |> reportFinding
        match severity,finding.Severity with
        | Error,_ -> Error
        | _, Error -> Error
        | _,_ -> Warning) Warning
