module Plainion.JekyllLint.UseCases.Rules

open Plainion.JekyllLint.Entities

let PageTitleTooLong severity maxLength page =
    let finding = finding 1 page severity
    match page.Header.Title with
    | None -> "Page has no title" |> finding |> Some
    | Some title when title.Length > maxLength -> sprintf "Title too long: %i > %i" title.Length maxLength |> finding |> Some
    | _ -> None
