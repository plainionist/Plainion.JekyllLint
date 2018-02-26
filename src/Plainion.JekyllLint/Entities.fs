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

