let bindingFile = "llvm.cs"

open System.IO
open System.Text.RegularExpressions

let bindingRegex = Regex("EntryPoint\s+\=\s+\"(?<func>LLVM[\d\w]+)\"", RegexOptions.Compiled)

let getBinding i line =
    let match' = bindingRegex.Match(line)
    if match'.Success then
        let funcName = match'.Groups.["func"].Value
        Some(i, funcName)
    else
        None

let defRegex = Regex("^LLVM[\d\w]+$", RegexOptions.Compiled ||| RegexOptions.Singleline)

let getExport line =
    let match' = defRegex.Match(line)
    if match'.Success then
        let funcName = match'.Value
        Some(funcName)
    else
        None

let checkExternals defFile =
    let bindingFile = Path.Combine(__SOURCE_DIRECTORY__, bindingFile)
    let bindingLines = File.ReadAllLines(bindingFile)
    let bindings =
        bindingLines
        |> Array.mapi(fun i line -> i, line)
        |> Seq.choose (fun (i, line) -> getBinding i line)
    
    let defLines = File.ReadAllLines(defFile)
    let defs =
        defLines
        |> Seq.choose getExport
        |> Set.ofSeq

    for lineNo, binding in bindings do
        if not(Set.contains binding defs) then
            eprintfn "%s [%d] is missing" binding lineNo
