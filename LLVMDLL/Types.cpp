#include "llvm/IR/DerivedTypes.h"
#include "llvm/IR/LLVMContext.h"
#include "llvm-c/Core.h"
#include "DllApi.h"

using namespace llvm;

LLVMDLL_FUNC(LLVMTypeRef) LLVMStructCreateEmptyTypeInContext(LLVMContextRef C){
	return wrap(StructType::create(*unwrap(C)));
}