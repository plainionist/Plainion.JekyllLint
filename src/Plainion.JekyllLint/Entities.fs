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
    Id : int
    Page : Page
    Severity : Severity
    Message : string
}

let finding id page severity message =
    { Id = id
      Page = page
      Severity = severity
      Message = message }

type DescriptionAttribute(value) =
    inherit Attribute()

    member this.Value = value

    