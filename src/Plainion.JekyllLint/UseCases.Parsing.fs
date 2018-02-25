module Plainion.JekyllLint.UseCases.Parsing

open System
open Plainion.JekyllLint.Entities

[<AutoOpen>]
module private Impl =
    let isFrontMatterSeparator (str:string) = str.Trim().Equals("---")

    // TODO: multi line front matter with "|"
    let getKeyValuePairs (lines:string seq) =
        lines
        |> Seq.map(fun line -> 
            let tokens = line.Split(':')
            tokens.[0].Trim().ToLower(),tokens.[1].Trim())


let GetHeader (lines:string seq) =
    let attributes =
        match lines |> List.ofSeq with
        | [] -> Map.empty
        | h::t when h |> isFrontMatterSeparator -> 
            t 
            |> Seq.takeWhile (isFrontMatterSeparator >> not)
            |> getKeyValuePairs
            |> Map.ofSeq
        | _ -> Map.empty

    {
        Title = attributes |> Map.tryFind "title"
        Attributes = attributes
    }

let CreatePage (location,lines:string seq) =
    let content =
        match lines |> List.ofSeq with
        | [] -> []
        | h::t when h |> isFrontMatterSeparator -> 
            t 
            |> Seq.skipWhile (isFrontMatterSeparator >> not) 
            |> Seq.skip 1 
            |> List.ofSeq
        | x -> x

    let header = lines |> GetHeader

    { 
        Location = location
        Header = header
        Content = content 
    }