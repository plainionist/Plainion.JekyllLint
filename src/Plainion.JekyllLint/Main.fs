module Plainion.JekyllLint.Main

open System.IO
open System
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases
open Plainion.JekyllLint.Gateways
open Plainion.JekyllLint.Gateways.Controller
open Plainion.JekyllLint.UseCases.Engine


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
    severityMapping : SeverityMapping }

let rec parseCommandLineRec args options = 
    match args with 
    | [] -> options  
    | "-h"::xs -> parseCommandLineRec xs {options with printHelp = true} 
    | "-error-to-warning"::xs -> parseCommandLineRec xs {options with severityMapping = ErrorToWarning} 
    | "-warning-to-error"::xs -> parseCommandLineRec xs {options with severityMapping = WarningToError} 
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
        severityMapping = AsIs}

    let options = parseCommandLineRec (argv |> List.ofArray) defaultOptions 

    let getPages = Storange.getAllPages Parsing.createPage
    let validatePage = Engine.validatePage Rules.All options.severityMapping

    if options.printHelp then
        usage ()
        0
    else    
        printfn "Working directory: %s" options.sources

        options.sources
        |> Controller.validate getPages validatePage 
        |> function | Error -> 1 | _ -> 0
