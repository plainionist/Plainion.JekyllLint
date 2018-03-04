module Plainion.JekyllLint.Entities

open System

type RuleId = | RuleId of string

let createRuleId id =
    id |> sprintf "JL%04i" |> RuleId

let printRuleId id =
    let (RuleId v) = id
    v

type Header = {
    Title : string option 
    Description : string option 
    NoWarn : RuleId list
    Attributes : Map<string,string>
}

type Page = {
    Location : string
    Header : Header
    Content : string list 
    ContentStartLine : int
}

type Image = {
    Source : string
    Alt : string option
    Title : string option
}

type SeverityInterpretation =
    | AsIs
    | WarningToError
    | ErrorToWarning

type Severity =
    | Warning 
    | Error

type Finding = {
    Id : RuleId
    Page : Page
    LineNumber : int
    Severity : Severity
    Message : string
}

type RuleAttribute(id:int) =
    inherit Attribute()

    member this.Id = id |> createRuleId

