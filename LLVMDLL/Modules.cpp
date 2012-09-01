#include "llvm/GlobalVariable.h"
#include "llvm-c/Core.h"

using namespace llvm;

extern "C" LLVMValueRef LLVMAddGlobalValue(LLVMModuleRef M, LLVMTypeRef Ty, 
								const char *Name, LLVMValueRef constant) {
  return wrap(new GlobalVariable(*unwrap(M), unwrap(Ty), false,
                                 GlobalValue::ExternalLinkage, 
								 unwrap<Constant>(constant), Name));
}