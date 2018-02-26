module Plainion.JekyllLint.UseCases.Rules

open Plainion.JekyllLint.Entities

[<Rule(1, "A page should have a title.")>]
let PageTitleMissing page =
    match page.Header.Title with
    | None -> "Page has no title" |> Some
    | _ -> None

[<Rule(2,"Titles which are too long might be shortened by search engines.")>]
let PageTitleTooLong maxLength page =
    match page.Header.Title with
    | Some title when title.Length > maxLength -> sprintf "Title too long: %i > %i" title.Length maxLength |> Some
    | _ -> None

[<Rule(3,"Content which is too short might be down ranked by search engines as not important enough.")>]
let ContentTooShort minLength page =
    let count = 
        page.Content
        |> Seq.map(fun line -> line.Length)
        |> Seq.sum

    if count < minLength then
        sprintf "Content too short: %i < %i" count minLength |> Some
    else
        None