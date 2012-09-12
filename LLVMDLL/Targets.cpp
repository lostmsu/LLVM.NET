#include "llvm-c/Target.h"
#include "DllApi.h"
//#include "stdlib.h"

LLVMDLL_FUNC(void) LLVMInitializeAllTargetsDynamicLibrary(){
	LLVMInitializeAllTargets();
}

LLVMDLL_FUNC(bool) LLVMInitializeNativeTargetDynamicLibrary(){
	LLVMBool success = LLVMInitializeNativeTarget();
	printf("initialization status: %d", success);
	return success;
}

LLVMDLL_FUNC(void) LLVMInitializeAllTargetInfosDynamicLibrary(){
	LLVMInitializeAllTargetInfos();
}

LLVMDLL_FUNC(void) LLVMInitializeAllTargetMCsDynamicLibrary(){
	LLVMInitializeAllTargetMCs();
}

LLVMDLL_FUNC(void) LLVMInitializeAllAsmPrintersDynamicLibrary() {
	LLVMInitializeAllAsmPrinters();
}

LLVMDLL_FUNC(void) LLVMInitializeAllAsmParsersDynamicLibrary() {
	LLVMInitializeAllAsmParsers();
}

LLVMDLL_FUNC(void) LLVMInitializeAllDisassemblersDynamicLibrary() {
	LLVMInitializeAllDisassemblers();
}
