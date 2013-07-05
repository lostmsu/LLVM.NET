#include "llvm/IR/Value.h"
#include "llvm-c/Core.h"
#include "DllApi.h"

using namespace llvm;

LLVMDLL_FUNC(void) LLVMDump(LLVMValueRef fun){
	unwrap(fun)->dump();
}