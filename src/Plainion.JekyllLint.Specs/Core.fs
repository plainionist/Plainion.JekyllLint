namespace Plainion.JekyllLint.Specs

open NUnit.Framework
open FsUnit
open Plainion.JekyllLint.Entities
open Plainion.JekyllLint.UseCases

[<AutoOpen>]
module internal Impl =
    let page lines = 
        ("",lines) |> Parsing.createPage
    
    let validate rule = 
        Engine.validatePage [ rule |> Rules.Compiler.compile, Warning ] Engine.AsIs
    
    let finding findings = 
        match findings |> List.ofSeq with
        | [] -> failwith "No finding reported"
        | [f] -> f.Id |> printRuleId,f.LineNumber,f.Message
        | _ -> failwith "More than one finding reported"

