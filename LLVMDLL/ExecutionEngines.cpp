#include "llvm/ExecutionEngine/ExecutionEngine.h"
#include "llvm-c/ExecutionEngine.h"
#include "DllApi.h"

using namespace llvm;

void* (*lazyFunction)(const char* );

void* LazyFunctionWrapper(const std::string& name) {
	return lazyFunction(name.c_str());
}

LLVMDLL_FUNC(void) LLVMSetLazyFunctionCallback(LLVMExecutionEngineRef EE, void* (*callback)(const char* )){
	if (!lazyFunction)
		lazyFunction = callback;
	return unwrap(EE)->InstallLazyFunctionCreator(LazyFunctionWrapper);
}