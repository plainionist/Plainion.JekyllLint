module Plainion.JekyllLint.UseCases.Engine

open System.Reflection
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Linq.RuntimeHelpers
open Plainion.JekyllLint.Entities

let compileRule expr =
    let rec getMethodInfo expr =
        match expr with
        | Call(_, methodInfo, _) -> methodInfo |> Some
        | Lambda(_, body) -> getMethodInfo body
        | Let(_, _, expr2) -> getMethodInfo expr2
        | _ -> None

    match getMethodInfo expr with
    | Some mi -> 
        let attr = mi.GetCustomAttribute(typeof<RuleAttribute>) :?> RuleAttribute
        attr.Id,(LeafExpressionConverter.EvaluateQuotation(expr) :?> (Page -> string list))
    | None -> failwithf "Could not extract id from: %A" expr

let validatePage rules page =
    let finding id severity msg =
        { Id = id
          Page = page
          Severity = severity
          Message = msg }
    
    rules
    |> Seq.filter(fun ((id,_),_) -> page.Header.NoWarn |> Seq.contains id |> not)
    |> Seq.map(fun ((id,rule),severity) -> page |> rule |> Seq.map (finding id severity))
    |> Seq.concat
    |> List.ofSeq


let reportFinding finding =
    printfn "%s: %s %s: %s" finding.Page.Location (finding.Severity.ToString()) (printRuleId finding.Id) finding.Message
