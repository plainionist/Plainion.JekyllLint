module Plainion.JekyllLint.UseCases.Parsing

open System
open Plainion.JekyllLint.Entities

[<AutoOpen>]
module private Impl =
    let isFrontMatterSeparator (str:string) = str.Trim().Equals("---")

let GetHeader (lines:string seq) =
    let attributes =
        match lines |> List.ofSeq with
        | [] -> Map.empty
        | h::t when h |> isFrontMatterSeparator -> 
            t 
            |> Seq.takeWhile (isFrontMatterSeparator >> not)
            |> Seq.map(fun l -> l.Split(':'))
            |> Seq.map(fun tokens -> tokens.[0].Trim().ToLower(),tokens.[1].Trim())
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