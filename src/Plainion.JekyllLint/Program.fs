module Plainion.JekyllLint.Main

open System.IO
open System
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases

let rules = 
    [
        Rules.PageTitleTooLong Warning 60
    ]

let allFiles dir =
    Directory.EnumerateFiles(dir,"*", SearchOption.AllDirectories)

let filterTextFiles file =
    let textExt = [ ".md"; ".txt" ]
    let inputExt = Path.GetExtension(file)

    if textExt |> Seq.exists(fun ext -> ext.Equals(inputExt, StringComparison.OrdinalIgnoreCase)) then
        true
    else
        printfn "Ignoring: %s" file
        false
    
let createPage file = 
    try
        (file,File.ReadAllLines(file))
        |> Parsing.CreatePage   
    with
        | ex -> failwithf "Failed to parse file: %s%s%s" file Environment.NewLine (ex.ToString())

let validatePage page =
    rules
    |> Seq.choose(fun rule -> page |> rule)

let reportFinding finding =
    printfn "%s: %s JL%04i: %s" finding.Page.Location (finding.Severity.ToString()) finding.Id finding.Message

[<EntryPoint>]
let main argv = 
    
    let cwd = 
        match argv |> List.ofArray with
        | [] -> Path.GetFullPath(".")
        | h::t -> h

    printfn "Working directory: %s" cwd

    cwd
    |> allFiles
    |> Seq.filter filterTextFiles
    |> Seq.map createPage
    |> Seq.collect validatePage
    |> Seq.iter reportFinding

    0 
