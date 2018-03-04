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
        attr.Id,(LeafExpressionConverter.EvaluateQuotation(expr) :?> (Page -> (int*string) list))
    | None -> failwithf "Could not extract id from: %A" expr

let validatePage rules page =
    let finding id severity (lineNo,msg) =
        { Id = id
          Page = page
          LineNumber = lineNo
          Severity = severity
          Message = msg }
    
    rules
    |> Seq.filter(fun ((id,_),_) -> page.Header.NoWarn |> Seq.contains id |> not)
    |> Seq.map(fun ((id,rule),severity) -> page |> rule |> Seq.map (finding id severity))
    |> Seq.concat
    |> List.ofSeq


let reportFinding severityInterpretation finding =
    let severity = 
        match severityInterpretation,finding.Severity with
        | AsIs,x -> x
        | WarningToError,_ -> Error
        | ErrorToWarning,_ -> Warning

    printfn "%s(%i,0): %s %s: %s" finding.Page.Location finding.LineNumber (severity.ToString()) (printRuleId finding.Id) finding.Message
    severity
