module Plainion.JekyllLint.Main

open System.IO
open System
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases
open Plainion.JekyllLint.Gateways

let rules = 
    [
        <@ Rules.TitleMissing                   @> |> Engine.compileRule, Error
        <@ Rules.TitleTooLong                   @> |> Engine.compileRule, Warning
        <@ Rules.ContentTooShort 2000           @> |> Engine.compileRule, Warning
        <@ Rules.DescriptionMissing             @> |> Engine.compileRule, Error
        <@ Rules.DescriptionLengthNotOptimal    @> |> Engine.compileRule, Warning
        <@ Rules.ImageHasNoAltText              @> |> Engine.compileRule, Error
        <@ Rules.ImageHasNoTitleText            @> |> Engine.compileRule, Warning
    ]

let usage() =
    printfn "Plainion.JekyllLint [Options] <folder to markdown files>"
    printfn ""
    printfn "Options:"
    printfn  "  -h                 - Prints this help"
    printfn  "  -error-to-warning  - handle all errors as warnings"
    printfn  "  -warning-to-error  - handle all warnings as errors"

type CommandLineOptions = {
    printHelp : bool
    sources : string
    severityInterpretation : SeverityInterpretation }

let rec parseCommandLineRec args options = 
    match args with 
    | [] -> options  
    | "-h"::xs -> parseCommandLineRec xs {options with printHelp = true} 
    | "-error-to-warning"::xs -> parseCommandLineRec xs {options with severityInterpretation = ErrorToWarning} 
    | "-warning-to-error"::xs -> parseCommandLineRec xs {options with severityInterpretation = WarningToError} 
    | x::xs ->
        if x.StartsWith("-") then
            printfn "Option '%s' is unrecognized" x
            parseCommandLineRec xs options 
        else
            parseCommandLineRec xs {options with sources = x} 
       
[<EntryPoint>]
let main argv = 
    let defaultOptions = {
        printHelp = false
        sources =  Path.GetFullPath(".") 
        severityInterpretation = AsIs}

    let options = parseCommandLineRec (argv |> List.ofArray) defaultOptions 

    if options.printHelp then
        usage ()
        0
    else    
        printfn "Working directory: %s" options.sources

        options.sources
        |> Pages.getAllPages Parsing.createPage
        |> Seq.collect (Engine.validatePage rules)
        |> Seq.fold(fun severity finding -> 
            match severity,(finding |> Engine.reportFinding options.severityInterpretation ) with
            | Error,_ -> Error
            | _, Error -> Error
            | _,_ -> Warning) Warning
        |> function | Error -> 1 | _ -> 0
