#include "llvm-c/Target.h"

extern "C" void LLVMInitializeAllTargetsDynamicLibrary(){
	LLVMInitializeAllTargets();
}

extern "C" bool LLVMInitializeNativeTargetDynamicLibrary(){
	return LLVMInitializeNativeTarget();
}

extern "C" void LLVMInitializeAllTargetInfosDynamicLibrary(){
	LLVMInitializeAllTargetInfos();
}

extern "C" void LLVMInitializeAllTargetMCsDynamicLibrary(){
	LLVMInitializeAllTargetMCs();
}

extern "C" void LLVMInitializeAllAsmPrintersDynamicLibrary() {
	LLVMInitializeAllAsmPrinters();
}

extern "C" void LLVMInitializeAllAsmParsersDynamicLibrary() {
	LLVMInitializeAllAsmParsers();
}

extern "C" void LLVMInitializeAllDisassemblersDynamicLibrary() {
	LLVMInitializeAllDisassemblers();
}
