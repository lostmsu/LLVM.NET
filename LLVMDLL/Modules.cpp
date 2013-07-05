#include "llvm/IR/GlobalVariable.h"
#include "llvm/IR/Module.h"
#include "llvm-c/Core.h"
#include "DllApi.h"

using namespace llvm;

LLVMDLL_FUNC(LLVMValueRef) LLVMAddGlobalValue(LLVMModuleRef M, LLVMTypeRef Ty, 
								const char *Name, LLVMValueRef constant) {
  return wrap(new GlobalVariable(*unwrap(M), unwrap(Ty), false,
                                 GlobalValue::ExternalLinkage, 
								 unwrap<Constant>(constant), Name));
}