module Plainion.JekyllLint.UseCases.Rules

open Plainion.JekyllLint.Entities

[<Description("A page should have a title.")>]
let PageTitleMissing severity page =
    let finding = finding 1 page severity
    match page.Header.Title with
    | None -> "Page has no title" |> finding |> Some
    | _ -> None

[<Description("Titles which are too long might be shortened by search engines.")>]
let PageTitleTooLong severity maxLength page =
    let finding = finding 2 page severity
    match page.Header.Title with
    | Some title when title.Length > maxLength -> sprintf "Title too long: %i > %i" title.Length maxLength |> finding |> Some
    | _ -> None

[<Description("Content which is too short might be down ranked by search engines as not important enough.")>]
let ContentTooShort severity minLength page =
    let finding = finding 3 page severity
    let count = 
        page.Content
        |> Seq.map(fun line -> line.Length)
        |> Seq.sum

    if count < minLength then
        sprintf "Content too short: %i < %i" count minLength |> finding |> Some
    else
        None