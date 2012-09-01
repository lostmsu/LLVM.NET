#include "llvm-c/Core.h"

using namespace llvm;

extern "C" void LLVMDump(LLVMValueRef fun){
	unwrap(fun)->dump();
}