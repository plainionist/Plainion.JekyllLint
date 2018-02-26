module Plainion.JekyllLint.UseCases.Parsing

open System
open Plainion.JekyllLint.Entities

[<AutoOpen>]
module private Impl =
    let isFrontMatterSeparator (str:string) = str.Trim().Equals("---")

    // TODO: multi line front matter with "|"
    let getKeyValuePairs (lines:string seq) =
        let rec readLines pairs multiLine (lines:string list)=
            match multiLine,lines with 
            | None,[] -> pairs
            | Some x,[] -> x::pairs
            | None,h::t ->
                match h.Split(':') |> Seq.map(fun t -> t.Trim()) |> List.ofSeq with
                | [] -> failwithf "Front matter line not separated with ':' : %s" h
                | [ x ] -> failwithf "Front matter has no value: %s" h
                | [ key; "|" ] -> t |> readLines pairs (Some(key,""))
                | key::value -> t |> readLines ((key,value |> String.concat " ")::pairs) None
            | Some(k,v),h::t when h.StartsWith("  ") -> t |> readLines pairs (Some(k,v + h.Trim()))
            | Some(k,v),h::t -> t |> readLines ((k,v + h.Trim())::pairs) None

        lines 
        |> List.ofSeq
        |> readLines [] None

let getHeader (lines:string seq) =
    let attributes =
        match lines |> List.ofSeq with
        | [] -> Map.empty
        | h::t when h |> isFrontMatterSeparator -> 
            t 
            |> Seq.takeWhile (isFrontMatterSeparator >> not)
            |> getKeyValuePairs
            |> Map.ofSeq
        | _ -> Map.empty

    let noWarn =
        let parseNoWarn (value:string) =
             value.Split([| ',';';' |]) 
             |> Seq.map(fun id -> id.Trim() |> RuleId)
             |> List.ofSeq

        attributes 
        |> Map.tryFind "lint-nowarn" |> Option.map parseNoWarn |> Option.defaultValue [] 

    {
        Title = attributes |> Map.tryFind "title"
        NoWarn = noWarn
        Attributes = attributes
    }

let createPage (location,lines:string seq) =
    let content =
        match lines |> List.ofSeq with
        | [] -> []
        | h::t when h |> isFrontMatterSeparator -> 
            t 
            |> Seq.skipWhile (isFrontMatterSeparator >> not) 
            |> Seq.skip 1 
            |> List.ofSeq
        | x -> x

    let header = lines |> getHeader

    { 
        Location = location
        Header = header
        Content = content 
    }