module Plainion.JekyllLint.Main

open Plainion.JekyllLint.UseCases
open Plainion.JekyllLint.Gateways
       
[<EntryPoint>]
let main argv = 
    let getPages = Storange.getAllPages Parsing.createPage
    let validatePages = Engine.validatePages getPages Rules.All

    let lines,ret =
        argv
        |> Controller.performRequest validatePages 

    lines
    |> Seq.iter (printfn "%s")

    ret