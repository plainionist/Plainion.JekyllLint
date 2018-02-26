module Plainion.JekyllLint.Main

open System.IO
open System
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases

let rules = 
    [
        <@ Rules.PageTitleMissing     @> |> compileRule, Error
        <@ Rules.PageTitleTooLong 60  @> |> compileRule, Warning
        <@ Rules.ContentTooShort 2000 @> |> compileRule, Warning
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
    let finding id severity msg =
        { Id = id
          Page = page
          Severity = severity
          Message = msg }
    
    rules
    |> Seq.choose(fun ((id,rule),severity) -> page |> rule |> Option.map (finding id severity))

let reportFinding finding =
    printfn "%s: %s %s: %s" finding.Page.Location (finding.Severity.ToString()) finding.Id finding.Message

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
