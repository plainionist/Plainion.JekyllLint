module Plainion.JekyllLint.Entities

open System
open Microsoft.FSharp.Reflection

type Header = {
    Title : string option 
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
    Id : string
    Page : Page
    Severity : Severity
    Message : string
}

type RuleAttribute(id,value) =
    inherit Attribute()

    member this.Id = id |> sprintf "JL%04i"
    member this.Value = value

open Microsoft.FSharp.Quotations.Patterns
open System.Reflection
open Microsoft.FSharp.Linq.RuntimeHelpers

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
