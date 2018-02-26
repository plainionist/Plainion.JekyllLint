module Plainion.JekyllLint.Main

open System.IO
open System
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases
open Plainion.JekyllLint.Gateways

let rules = 
    [
        <@ Rules.PageTitleMissing     @> |> Engine.compileRule, Error
        <@ Rules.PageTitleTooLong 60  @> |> Engine.compileRule, Warning
        <@ Rules.ContentTooShort 2000 @> |> Engine.compileRule, Warning
    ]

[<EntryPoint>]
let main argv = 
    
    let cwd = 
        match argv |> List.ofArray with
        | [] -> Path.GetFullPath(".")
        | h::t -> h

    printfn "Working directory: %s" cwd

    cwd
    |> Pages.getAllPages Parsing.createPage
    |> Seq.collect (Engine.validatePage rules)
    |> Seq.iter Engine.reportFinding

    0 
