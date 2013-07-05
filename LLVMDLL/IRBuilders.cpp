#include "llvm/IR/LLVMContext.h"
#include "llvm/IR/IRBuilder.h"
#include "llvm-c/Core.h"
#include "DllApi.h"

using namespace llvm;

LLVMDLL_FUNC(LLVMContextRef) LLVMGetBuilderContext(LLVMBuilderRef builder){
	return wrap(&unwrap(builder)->getContext());
}