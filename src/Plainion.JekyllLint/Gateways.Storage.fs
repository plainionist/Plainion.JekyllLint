module Plainion.JekyllLint.Gateways.Storange

open System
open System.IO

let createPage creator file = 
    try
        (file,File.ReadAllLines(file))
        |> creator   
    with
        | ex -> failwithf "Failed to parse file: %s%s%s" file Environment.NewLine (ex.ToString())

let getAllPages creator dir =
    let filterTextFiles file =
        let textExt = [ ".md"; ".txt" ]
        let inputExt = Path.GetExtension(file)

        if textExt |> Seq.exists(fun ext -> ext.Equals(inputExt, StringComparison.OrdinalIgnoreCase)) then
            true
        else
            printfn "Ignoring: %s" file
            false
    
    Directory.EnumerateFiles(dir,"*", SearchOption.AllDirectories)
    |> Seq.filter filterTextFiles
    |> Seq.map (createPage creator)

