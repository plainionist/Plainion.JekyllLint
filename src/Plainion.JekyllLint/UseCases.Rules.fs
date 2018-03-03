﻿module Plainion.JekyllLint.UseCases.Rules

open Plainion.JekyllLint.Entities

let private toList x = [ x ]

[<Rule(1)>]
let TitleMissing page =
    match page.Header.Title with
    | None -> [ "Page has no title" ]
    | _ -> []

[<Rule(2)>]
let TitleTooLong page =
    let maxLength = 60
    match page.Header.Title with
    | Some title when title.Length > maxLength -> sprintf "Title too long: %i chars > %i chars" title.Length maxLength |> toList
    | _ -> []

[<Rule(3)>]
let ContentTooShort minLength page =
    let count = 
        page.Content
        |> Seq.map(fun line -> line.Split([|' ';'\t'|]).Length)
        |> Seq.sum

    if count < minLength then
        sprintf "Content too short: %i words < %i words" count minLength |> toList
    else
        []

[<Rule(4)>]
let DescriptionMissing page =
    match page.Header.Description with
    | None -> "Page has no description" |> toList
    | _ -> []

[<Rule(5)>]
let DescriptionLengthNotOptimal page =
    let minLength = 50
    let maxLength = 300
    match page.Header.Description with
    | Some desc when desc.Length < minLength -> sprintf "Description too short: %i chars < %i chars" desc.Length minLength |> toList
    | Some desc when desc.Length > maxLength -> sprintf "Description too long: %i chars > %i chars" desc.Length maxLength |> toList
    | _ -> []

let private verifyLines verify page =
    page.Content
    |> Seq.mapi(fun lineNo line -> line |> Parsing.tryGetImage |> Option.bind (verify (page.ContentStartLine + lineNo)))
    |> Seq.choose id
    |> List.ofSeq

[<Rule(6)>]
let ImageHasNoAltText page =
    let verifyAltText lineNo image =
        match image.Alt with
        | None -> lineNo |> sprintf "Image in line %i has no alt text" |> Some
        | Some _ -> None

    page |> verifyLines verifyAltText

[<Rule(7)>]
let ImageHasNoTitleText page =
    let verifyTitleText lineNo image =
        match image.Alt with
        | None -> lineNo |> sprintf "Image in line %i has no title text" |> Some
        | Some _ -> None

    page |> verifyLines verifyTitleText
