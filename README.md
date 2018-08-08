LLVM.NET
========

C# wrapper: [![nuget](https://img.shields.io/nuget/v/LLVM.svg)](https://www.nuget.org/packages/LLVM/)
LLVM binaries: [![nuget](https://img.shields.io/nuget/v/LLVM.NativeLibrary.svg)](https://www.nuget.org/packages/LLVM.NativeLibrary/)

Mono/.NET bindings to LLVM compiler framework

## Building LLVM.NativeLibrary
- download and unpack LLVM-3.3 source code from the official site into ../3.3/source
- run CMake to configure build into ../3.3/AMD64
- only LLVM_BUILD_RUNTIME and LLVM_INCLUDE_RUNTIME are required
- set LLVM_TARGETS_TO_BUILD to X86 (if you need to build all targets, LLVMDLL libs list must be modified)
- generate project for 64-bit Visual Studio tools
- build Release or Debug
- repeat all steps for ../3.3/IA32 folder and 32-bit Visual Studio tools

after that you should be able to build LLVM.NativeLibrary project from Visual Studio for either x64 or x86



