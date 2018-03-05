module Plainion.JekyllLint.Main

open Plainion.JekyllLint.UseCases
open Plainion.JekyllLint.Gateways
       
[<EntryPoint>]
let main argv = 
    let getPages = Storange.getAllPages Parsing.createPage
    let validatePage = Engine.validatePage Rules.All

    let lines,ret =
        argv
        |> Controller.performRequest getPages validatePage 

    lines
    |> Seq.iter (printfn "%s")

    ret