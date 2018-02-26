module Plainion.JekyllLint.Entities

open System
open System.Reflection
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Linq.RuntimeHelpers

type RuleId = | RuleId of string

let createRuleId id =
    id |> sprintf "JL%04i" |> RuleId

let printRuleId id =
    let (RuleId v) = id
    v

type Header = {
    Title : string option 
    NoWarn : RuleId list
    Attributes : Map<string,string>
}

type Page = {
    Location : string
    Header : Header
    Content : string list 
}

type Severity =
    | Warning 
    | Error

type Finding = {
    Id : RuleId
    Page : Page
    Severity : Severity
    Message : string
}

type RuleAttribute(id:int,value) =
    inherit Attribute()

    member this.Id = id |> createRuleId
    member this.Value = value

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
        attr.Id,(LeafExpressionConverter.EvaluateQuotation(expr) :?> (Page -> string option))
    | None -> failwithf "Could not extract id from: %A" expr
