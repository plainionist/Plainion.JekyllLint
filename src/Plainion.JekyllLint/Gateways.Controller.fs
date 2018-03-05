module Plainion.JekyllLint.Gateways.Controller

open System
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases.Engine
open System.IO

[<AutoOpen>]
module internal Presenter =
    let reportFinding finding =
        sprintf "%s(%i,0): %s %s: %s" finding.Page.Location finding.LineNumber (finding.Severity.ToString()) (printRuleId finding.Id) finding.Message

    let reportUsage() =
        [
            "Plainion.JekyllLint [Options] <folder to markdown files>"
            ""
            "Options:"
            "  -h                 - Prints this help"
            "  -error-to-warning  - handle all errors as warnings"
            "  -warning-to-error  - handle all warnings as errors"
        ]

[<AutoOpen>]
module internal Impl =
    let validate getPages validatePage sources =
        let findings = 
            sources
            |> getPages
            |> Seq.collect validatePage
            |> List.ofSeq
    
        let lines = 
            findings
            |> List.map reportFinding
    
        let ret = 
            findings
            |> Seq.map(fun f -> f.Severity)
            |> aggregateSeverity
            |> function | Error -> 1 | _ -> 0

        lines,ret

    type Request = {
        printHelp : bool
        sources : string
        severityMapping : SeverityMapping }

    let rec parseCommandLineRec args request = 
        match args with 
        | [] -> request  
        | "-h"::xs -> parseCommandLineRec xs {request with printHelp = true} 
        | "-error-to-warning"::xs -> parseCommandLineRec xs {request with severityMapping = ErrorToWarning} 
        | "-warning-to-error"::xs -> parseCommandLineRec xs {request with severityMapping = WarningToError} 
        | x::xs ->
            if x.StartsWith("-") then
                printfn "Option '%s' is unrecognized" x
                parseCommandLineRec xs request 
            else
                parseCommandLineRec xs {request with sources = x} 

let performRequest getPages validatePage argv =
    let defaultRequest = {
        printHelp = false
        sources =  Path.GetFullPath(".") 
        severityMapping = AsIs}

    let options = parseCommandLineRec (argv |> List.ofArray) defaultRequest 

    if options.printHelp then
        reportUsage(),0
    else    
        printfn "Working directory: %s" options.sources

        options.sources
        |> validate getPages (validatePage options.severityMapping)
