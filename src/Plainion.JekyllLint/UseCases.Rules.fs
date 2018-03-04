module Plainion.JekyllLint.UseCases.Rules

open System
open Plainion.JekyllLint.Entities

let private toList x = [ x ]
let private fileIssue msg = 0,msg
let private lineIssue lineNo msg = lineNo,msg

type RuleAttribute(id:int) =
    inherit Attribute()

    member this.Id = id |> createRuleId

[<Rule(1)>]
let TitleMissing page =
    match page.Header.Title with
    | None -> "Page has no title" |> fileIssue |> toList
    | _ -> []

[<Rule(2)>]
let TitleTooLong page =
    let maxLength = 60
    match page.Header.Title with
    | Some title when title.Length > maxLength -> sprintf "Title too long: %i chars > %i chars" title.Length maxLength |> fileIssue |> toList
    | _ -> []

[<Rule(3)>]
let ContentTooShort minLength page =
    let count = 
        page.Content
        |> Seq.map(fun line -> line.Split([|' ';'\t'|]).Length)
        |> Seq.sum

    if count < minLength then
        sprintf "Content too short: %i words < %i words" count minLength |> fileIssue |> toList
    else
        []

[<Rule(4)>]
let DescriptionMissing page =
    match page.Header.Description with
    | None -> "Page has no description" |> fileIssue |> toList
    | _ -> []

[<Rule(5)>]
let DescriptionLengthNotOptimal page =
    let minLength = 50
    let maxLength = 300
    match page.Header.Description with
    | Some desc when desc.Length < minLength -> sprintf "Description too short: %i chars < %i chars" desc.Length minLength |> fileIssue |> toList
    | Some desc when desc.Length > maxLength -> sprintf "Description too long: %i chars > %i chars" desc.Length maxLength |> fileIssue |> toList
    | _ -> []

let private verifyLines verify page =
    page.Content
    |> Seq.mapi(fun lineNo line -> line |> Parsing.tryGetImage |> Option.bind verify |> Option.map (lineIssue (page.ContentStartLine + lineNo)))
    |> Seq.choose id
    |> List.ofSeq

[<Rule(6)>]
let ImageHasNoAltText page =
    let verifyAltText image =
        match image.Alt with
        | None -> "Image has no alt text" |> Some
        | Some _ -> None

    page |> verifyLines verifyAltText

[<Rule(7)>]
let ImageHasNoTitleText page =
    let verifyTitleText image =
        match image.Title with
        | None -> "Image has no title text" |> Some
        | Some _ -> None

    page |> verifyLines verifyTitleText

module Compiler = 
    open System.Reflection
    open Microsoft.FSharp.Quotations.Patterns
    open Microsoft.FSharp.Linq.RuntimeHelpers

    let compile expr =
        let rec getMethodInfo expr =
            match expr with
            | Call(_, methodInfo, _) -> methodInfo |> Some
            | Lambda(_, body) -> getMethodInfo body
            | Let(_, _, expr2) -> getMethodInfo expr2
            | _ -> None

        match getMethodInfo expr with
        | Some mi -> 
            let attr = mi.GetCustomAttribute(typeof<RuleAttribute>) :?> RuleAttribute
            attr.Id,(LeafExpressionConverter.EvaluateQuotation(expr) :?> (Page -> (int*string) list))
        | None -> failwithf "Could not extract id from: %A" expr

let All = 
    [
        <@ TitleMissing                   @> |> Compiler.compile, Error
        <@ TitleTooLong                   @> |> Compiler.compile, Warning
        <@ ContentTooShort 2000           @> |> Compiler.compile, Warning
        <@ DescriptionMissing             @> |> Compiler.compile, Error
        <@ DescriptionLengthNotOptimal    @> |> Compiler.compile, Warning
        <@ ImageHasNoAltText              @> |> Compiler.compile, Error
        <@ ImageHasNoTitleText            @> |> Compiler.compile, Warning
    ]
