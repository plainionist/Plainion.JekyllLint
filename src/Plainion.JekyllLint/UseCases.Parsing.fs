module Plainion.JekyllLint.UseCases.Parsing

open System
open Plainion.JekyllLint.Entities
open System.Text.RegularExpressions
open System.Collections.Generic

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
        Description = attributes |> Map.tryFind "description"
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

    let contentStart = 
        match content with
        | [] -> 0
        | h::t -> lines |> Seq.findIndex(fun l -> h = l)

    { 
        Location = location
        Header = header
        Content = content 
        ContentStartLine = contentStart + 1 // start counting with 1
    }

let markdownImage = new Regex("!\[(.*)\]\((.*)\)")
let urlWithTitle = new Regex("^(.*)\s+\"(.*)\"$")
let htmlImage = new Regex("<img(\s+[a-zA-Z0-9-]+=\"[^\"]*\")*")
let htmlAttributes = new Regex("\s+([a-zA-Z0-9-]+)=\"(.*)\"")

// syntax:
// ![alt text](url "title")
// <img src="" alt="" title=""/>
// https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet#images
let tryGetImage line =
    let md = markdownImage.Match(line)
    if md.Success then
        let altText = md.Groups.[1].Value
        let url,title =
            let urlText = md.Groups.[2].Value
            let md = urlWithTitle.Match(urlText)
            if md.Success then
                md.Groups.[1].Value,(md.Groups.[2].Value.Trim([|'"'|]) |> Some)
            else
                urlText,None

        { Alt = if String.IsNullOrWhiteSpace(altText) then None else altText |> Some
          Title = title
          Source = url } |> Some
    else
        let md1 = htmlImage.Match(line)
        if md1.Success then
            let attributes =
                md1.Groups.[1].Captures
                |> Seq.cast<Capture>
                |> Seq.map(fun g -> 
                    let md = htmlAttributes.Match(g.Value)
                    md.Groups.[1].Value,md.Groups.[2].Value)
                |> Map.ofSeq
            { Alt = attributes |> Map.tryFind("alt")
              Title = attributes |> Map.tryFind("title")
              Source = attributes |> Map.tryFind("src") |> Option.defaultValue ""
            } |> Some
        else 
            None
