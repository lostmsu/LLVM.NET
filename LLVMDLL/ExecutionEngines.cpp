#include "llvm/ExecutionEngine/ExecutionEngine.h"
#include "llvm-c/ExecutionEngine.h"

using namespace llvm;

void* (*lazyFunction)(const char* );

void* LazyFunctionWrapper(const std::string& name) {
	return lazyFunction(name.c_str());
}

void LLVMSetLazyFunctionCallback(LLVMExecutionEngineRef EE, void* (*callback)(const char* )){
	if (!lazyFunction)
		lazyFunction = callback;
	return unwrap(EE)->InstallLazyFunctionCreator(LazyFunctionWrapper);
}