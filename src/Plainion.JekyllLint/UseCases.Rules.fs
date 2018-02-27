﻿module Plainion.JekyllLint.UseCases.Rules

open Plainion.JekyllLint.Entities

[<Rule(1, "A page should have a title.")>]
let TitleMissing page =
    match page.Header.Title with
    | None -> "Page has no title" |> Some
    | _ -> None

[<Rule(2,"Titles which are too long might be shortened by search engines. See also: https://moz.com/learn/seo/title-tag")>]
let TitleTooLong page =
    let maxLength = 60
    match page.Header.Title with
    | Some title when title.Length > maxLength -> sprintf "Title too long: %i chars > %i chars" title.Length maxLength |> Some
    | _ -> None

[<Rule(3,"Content which is too short might be down ranked by search engines as not important enough.")>]
let ContentTooShort minLength page =
    let count = 
        page.Content
        |> Seq.map(fun line -> line.Split([|' ';'\t'|]).Length)
        |> Seq.sum

    if count < minLength then
        sprintf "Content too short: %i words < %i words" count minLength |> Some
    else
        None

[<Rule(4, "A page should have a description. It will be included in the meta tag if you have jekyll-seo plug-in configured.")>]
let DescriptionMissing page =
    match page.Header.Description with
    | None -> "Page has no description" |> Some
    | _ -> None

[<Rule(5,"Descriptions which are too long might be shortened by search engines. See also: https://moz.com/learn/seo/meta-description")>]
let DescriptionLengthNotOptimal page =
    let minLength = 50
    let maxLength = 300
    match page.Header.Description with
    | Some desc when desc.Length < minLength -> sprintf "Description too short: %i chars < %i chars" desc.Length minLength |> Some
    | Some desc when desc.Length > maxLength -> sprintf "Description too long: %i chars > %i chars" desc.Length maxLength |> Some
    | _ -> None
