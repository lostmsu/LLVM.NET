#include <string>

#include "llvm-c/Core.h"
#include "llvm/IR/Value.h"
#include "llvm/Support/raw_ostream.h"

#include <comutil.h>

#include "DllApi.h"

BSTR ANSItoBSTR(const char* input)
{
	BSTR result = NULL;
	int lenA = lstrlenA(input);
	int lenW = ::MultiByteToWideChar(CP_ACP, 0, input, lenA, NULL, 0);
	if (lenW > 0)
	{
		result = ::SysAllocStringLen(0, lenW);
		::MultiByteToWideChar(CP_ACP, 0, input, lenA, result, lenW);
	}
	return result;
}

LLVMDLL_FUNC(BSTR) LLVMPrint(LLVMValueRef value){
	std::string out;
	llvm::raw_string_ostream ostream(out);
	llvm::Value* unwrapped = llvm::unwrap(value);
	unwrapped->print(ostream);
	const char* result = ostream.str().c_str();
	return ANSItoBSTR(result);
}

LLVMDLL_FUNC(BSTR) LLVMGetValueNameAsBSTR(LLVMValueRef value){
	const char* name = LLVMGetValueName(value);
	return ANSItoBSTR(name);
}