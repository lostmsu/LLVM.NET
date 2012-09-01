#include "llvm-c/Core.h"

using namespace llvm;

extern "C" LLVMTypeRef LLVMStructCreateEmptyTypeInContext(LLVMContextRef C){
	return wrap(StructType::create(*unwrap(C)));
}