module Plainion.JekyllLint.Entities

open System

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

let getId x =

    ""