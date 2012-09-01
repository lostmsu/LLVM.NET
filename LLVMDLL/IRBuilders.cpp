#include "llvm-c/Core.h"

using namespace llvm;

extern "C" LLVMContextRef LLVMGetBuilderContext(LLVMBuilderRef builder){
	return wrap(&unwrap(builder)->getContext());
}