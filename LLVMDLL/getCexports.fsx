let wrappers =
    [   // ExecutionEngines
        "LazyFunctionWrapper";
        "LLVMSetLazyFunctionCallback";

        // Modules
        "LLVMAddGlobalValue";

        // Targets
        "LLVMInitializeAllTargetsDynamicLibrary";
        "LLVMInitializeNativeTargetDynamicLibrary";

        // GC
        "LLVMRegisterExternalGC";
        "LLVMCreateExternalGC";

        // IRBuilders
        "LLVMGetBuilderContext";

        // Types
        "LLVMStructCreateEmptyTypeInContext";
        ]
    |> (fun w -> System.String.Join("\n", w))

let ignoreHeaders =
    [   "EnhancedDisassembly.h";
        "Disassembler.h";
        "Initialization.h"; ]
    |> (fun h -> System.Collections.Generic.HashSet(h))

let dllFunctionSuffix = "DynamicLibrary"

let dllFunctions =
    [   "LLVMInitializeAllTargetInfos";
        "LLVMInitializeAllTargets";
        "LLVMInitializeAllTargetMCs";
        "LLVMInitializeAllAsmPrinters";
        "LLVMInitializeAllAsmParsers";
        "LLVMInitializeAllDisassemblers";
        "LLVMInitializeNativeTarget"; ]
    |> (fun h -> System.Collections.Generic.HashSet(h))

let header = @"EXPORTS

; wrappers
"               + wrappers + "\n\n"

open System
open System.IO

let llvmCincludeDir =
    let llvmBase = Environment.GetEnvironmentVariable("LLVM_BASE")
    Path.Combine((if llvmBase = null then "." else llvmBase), "include", "llvm-c")

let headerFiles = Directory.GetFiles(llvmCincludeDir, "*.h")

let defs = Text.StringBuilder()

open System.Text.RegularExpressions

let llvmDefinitionRegex = Regex("\S\s+\*?(?'func'LLVM[\w\d]+)\(", RegexOptions.Compiled)

let getLLVMdefinition line =
    let item = llvmDefinitionRegex.Match(line)
    if not(line.StartsWith("#")) && item.Success then
        let funcName = item.Groups.["func"].Value
        Some(line,
                if dllFunctions.Contains funcName then
                    funcName + dllFunctionSuffix
                else funcName)
    else
        None

let appendEntry(origLine, funcName) =
    defs//.Append("; ").AppendLine(origLine)
        .AppendLine(funcName) |> ignore

let appendFileEntries fileName =
    let shortName = Path.GetFileName fileName
    if ignoreHeaders.Contains shortName then ()
    else
    defs.Append("; ").AppendLine(shortName).AppendLine() |> ignore

    File.ReadAllLines(fileName)
    |> Seq.choose getLLVMdefinition
    |> Seq.iter appendEntry

    defs.AppendLine() |> ignore

let writeExports target =
    defs.Append(header) |> ignore

    headerFiles
    |> Seq.iter appendFileEntries

    File.WriteAllText(target, defs.ToString())
