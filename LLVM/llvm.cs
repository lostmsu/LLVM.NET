using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace LLVM
{
	using CC = System.Runtime.InteropServices.CallingConvention;
    using LLVM.GarbageCollection.NativeGlue;

	static class llvm
	{
		const string llvmdll = "LLVM-3.3";

		#region Types
		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMInt32TypeInContext")]
		public static extern IntPtr GetInt32(IntPtr context);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMIntTypeInContext")]
		public static extern IntPtr GetInt(IntPtr context, int bits);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMFloatTypeInContext")]
		public static extern IntPtr GetFloat(IntPtr context);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMDoubleTypeInContext")]
		public static extern IntPtr GetDouble(IntPtr context);


		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMVoidTypeInContext")]
		public static extern IntPtr GetVoid(IntPtr context);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMGetTypeKind")]
		public static extern TypeKind GetTypeKind(IntPtr typeref);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMPointerType")]
		public static extern IntPtr GetPointerType(IntPtr valueType, int addressSpace);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMFunctionType")]
		public static extern IntPtr FunctionType(IntPtr ret, IntPtr[] args, int argcount, bool vararg);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMCountParamTypes")]
		public static extern int GetArgumentCount(IntPtr functionType);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMGetParamTypes")]
		public static extern void GetArgumentTypes(IntPtr functionType, IntPtr[] types);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMGetReturnType")]
		public static extern IntPtr GetReturnType(IntPtr functionType);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMArrayType")]
		public static extern IntPtr ArrayType(IntPtr elemType, int elemCount);

		#region Structs
		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMStructTypeInContext")]
		public static extern IntPtr StructType(IntPtr context, IntPtr[] elements, int elemcount, bool packed);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMStructCreateNamed")]
		public static extern IntPtr StructType(IntPtr context, string name);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMStructCreateEmptyTypeInContext")]
		public static extern IntPtr StructCreateEmptyType(IntPtr context);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint="LLVMStructSetBody")]
		public static extern void StructSetBody(IntPtr structType, IntPtr[] elementTypes, int elementCount, bool packed);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMIsPackedStruct")]
		public static extern bool IsPackedStruct(IntPtr structType);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMIsOpaqueStruct")]
		public static extern bool IsOpaqueStruct(IntPtr structType);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMCountStructElementTypes")]
		public static extern uint StructFieldCount(IntPtr structType);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMGetStructElementTypes")]
		public static extern void StructElements(IntPtr structType, IntPtr[] types);
		#endregion

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMTypeOf")]
		public static extern IntPtr TypeOf(IntPtr value);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMGetElementType")]
		public static extern IntPtr GetElementType(IntPtr type);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMGetIntTypeWidth")]
		public static extern int GetWidth(IntPtr inttype);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMAlignOf")]
		public static extern IntPtr AlignOf(IntPtr typeref);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMSizeOf")]
		public static extern IntPtr SizeOf(IntPtr typeref);
		#endregion

		#region Modules
		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMModuleCreateWithNameInContext")]
		public static extern IntPtr CreateModule(string name, IntPtr context);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMAddFunction")]
		public static extern IntPtr CreateFunction(IntPtr module, string name, IntPtr type);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMAddGlobal")]
		public static extern IntPtr AddGlobal(IntPtr module, IntPtr type, string name);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMAddGlobalValue")]
		public static extern IntPtr AddGlobal(IntPtr module,
			IntPtr type, string name, IntPtr constant);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMGetNamedGlobal")]
		public static extern IntPtr GetNamedGlobal(IntPtr module, string name);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMGetModuleContext")]
		public static extern IntPtr GetContext(IntPtr module);
		#endregion

		#region Uility
		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMInitializeNativeTargetDynamicLibrary")]
		public static extern bool InitializeNative();

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMInitializeAllTargetsDynamicLibrary")]
		public static extern void InitializeAllTargets();

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMGetGlobalContext")]
		public static extern IntPtr GetGlobalContext();

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMCreateExecutionEngineForModule")]
		public static extern bool CreateExecutionEngine(out IntPtr engine, IntPtr module, out string error);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMAppendBasicBlockInContext")]
		public static extern IntPtr CreateBlock(IntPtr context, IntPtr func, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMCreateBuilderInContext")]
		public static extern IntPtr CreateBuilder(IntPtr context);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMGetBuilderContext")]
		public static extern IntPtr GetBuilderContext(IntPtr builder);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMPositionBuilderAtEnd")]
		public static extern void PointToEnd(IntPtr instructionBuilder, IntPtr block);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMGetPointerToGlobal")]
		public static extern IntPtr GetPointer(IntPtr executionEngine, IntPtr globalval);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMGetPointerToFunction")]
		public static extern IntPtr GetPointerToFunction(IntPtr executionEngine, IntPtr globalval);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMSetLazyFunctionCallback")]
		public static extern void SetLazyFunctionCallback(IntPtr executionEngine, LazyFunctionLoader loader);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMGetParam")]
		public static extern IntPtr GetParameter(IntPtr function, int index);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMDumpValue")]
		public static extern void Dump(IntPtr valueref);
		#endregion

		#region Constants
		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMGetUndef")]
		public static extern IntPtr GetUndefinedValue(IntPtr type);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMConstNull")]
		public static extern IntPtr GetZero(IntPtr type);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMConstInt")]
		public static extern IntPtr Constant(IntPtr typeref, ulong value, bool sign);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMConstIntToPtr")]
		public static extern IntPtr ToPointer(IntPtr value, IntPtr targetType);
		#endregion

		#region Functions
		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMSetFunctionCallConv")]
		public static extern void SetCallingConvention(IntPtr func, CallingConvention conv);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMGetFunctionCallConv")]
		public static extern CallingConvention GetCallingConvention(IntPtr func);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMSetInstructionCallConv")]
		public static extern void SetInstructionCallingConvention(IntPtr func, CallingConvention conv);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMGetInstructionCallConv")]
		public static extern CallingConvention GetInstructionCallingConvention(IntPtr func);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMIsTailCall")]
		public static extern bool IsTailCall(IntPtr call);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMSetTailCall")]
		public static extern void SetTailCall(IntPtr call, bool value);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMGetNamedFunction")]
		public static extern IntPtr GetFunction(IntPtr module, string name);
		#endregion

		#region Garbage Collection
		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMSetGC")]
		public static extern void SetGC(IntPtr function, string name);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMCreateExternalGC")]
		public static extern IntPtr CreateGC(ExternalGCInfo gcinfo);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMRegisterExternalGC")]
		public static extern IntPtr RegisterGC(IntPtr name, IntPtr descr, GCStrategyConstructor ctor);
		#endregion Garbage Collection

		#region Emit
		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildICmp")]
		public static extern IntPtr EmitCompare(IntPtr ibuilder, IntegerComparison comparison, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildIsNull")]
		public static extern IntPtr EmitIsNull(IntPtr ibuilder, IntPtr value, string name);

		#region Control Flow
		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildCondBr")]
		public static extern IntPtr EmitIf(IntPtr ibuilder, IntPtr cond, IntPtr ontrue, IntPtr onfalse);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMBuildSwitch")]
		public static extern IntPtr EmitSwitch(IntPtr ibuilder, IntPtr value, IntPtr elseCase, int caseCount);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMAddCase")]
		public static extern IntPtr SwitchAdd(IntPtr @switch, IntPtr value, IntPtr target);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildBr")]
		public static extern IntPtr EmitGoTo(IntPtr ibuilder, IntPtr targetBlock);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildCall")]
		public static extern IntPtr EmitCall(IntPtr ibuilder, IntPtr func, IntPtr[] args, int argc, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildRet")]
		public static extern IntPtr EmitReturn(IntPtr instructionBuilder, IntPtr value);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildRetVoid")]
		public static extern IntPtr EmitReturn(IntPtr instructionBuilder);
		#endregion

		#region Arithmetics
		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildSub")]
		public static extern IntPtr EmitSubtract(IntPtr ibuilder, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildAdd")]
		public static extern IntPtr EmitAdd(IntPtr ibuilder, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildFAdd")]
		public static extern IntPtr EmitAddFloat(IntPtr ibuilder, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildMul")]
		public static extern IntPtr EmitMultiply(IntPtr ibuilder, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildUDiv")]
		public static extern IntPtr EmitDivideUnsigned(IntPtr ibuilder, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildSDiv")]
		public static extern IntPtr EmitDivideSigned(IntPtr ibuilder, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildURem")]
		public static extern IntPtr EmitReminderUnsigned(IntPtr ibuilder, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildSRem")]
		public static extern IntPtr EmitReminderSigned(IntPtr ibuilder, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildNeg")]
		public static extern IntPtr EmitNegate(IntPtr ibuilder, IntPtr value, string name);
		#endregion

		#region Logics
		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildAnd")]
		public static extern IntPtr EmitAnd(IntPtr ibuilder, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildOr")]
		public static extern IntPtr EmitOr(IntPtr ibuilder, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildXor")]
		public static extern IntPtr EmitXor(IntPtr ibuilder, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildNot")]
		public static extern IntPtr EmitNot(IntPtr ibuilder, IntPtr value, string name);
		#endregion

		#region Shifts
		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildShl")]
		public static extern IntPtr EmitShiftLeft(IntPtr ibuilder, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildLShr")]
		public static extern IntPtr EmitShiftRightUnsigned(IntPtr ibuilder, IntPtr left, IntPtr right, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildAShr")]
		public static extern IntPtr EmitShiftRightSigned(IntPtr ibuilder, IntPtr left, IntPtr right, string name);
		#endregion

		#region Memory
		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildAlloca")]
		public static extern IntPtr EmitStackAlloc(IntPtr ibuilder, IntPtr typeref, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildArrayAlloca")]
		public static extern IntPtr EmitStackAlloc(IntPtr ibuilder, IntPtr typeref, IntPtr size, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildStore")]
		public static extern IntPtr EmitStore(IntPtr ibuilder, IntPtr value, IntPtr pointer);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildLoad")]
		public static extern IntPtr EmitLoad(IntPtr ibuilder, IntPtr pointer, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildStructGEP")]
		public static extern IntPtr EmitStructElementPointer(IntPtr ibuilder, IntPtr valueref, int index, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildGEP")]
		public static extern IntPtr EmitGetElementPointer(IntPtr ibuilder, IntPtr value, IntPtr[] offsets, int count, string name);
		#endregion

		#region Conversions
		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildTrunc")]
		public static extern IntPtr EmitTrunc(IntPtr ibuilder, IntPtr value, IntPtr destType, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildZExt")]
		public static extern IntPtr EmitZeroExtend(IntPtr ibuilder, IntPtr value, IntPtr destType, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildSExt")]
		public static extern IntPtr EmitSignExtend(IntPtr ibuilder, IntPtr value, IntPtr destType, string name);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMBuildPointerCast")]
		public static extern IntPtr EmitPointerCast(IntPtr ibuilder, IntPtr value, IntPtr destType, string name);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMBuildBitCast")]
		public static extern IntPtr EmitBitCast(IntPtr ibuilder, IntPtr value, IntPtr destType, string name);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMBuildIntToPtr")]
		public static extern IntPtr EmitIntToPtr(IntPtr ibuilder, IntPtr value, IntPtr destType, string name);

		[DllImport(llvmdll, CallingConvention = CC.Cdecl, EntryPoint = "LLVMBuildPtrToInt")]
		public static extern IntPtr EmitPtrToInt(IntPtr ibuilder, IntPtr value, IntPtr destType, string name);
		#endregion

		#region Structures
		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildExtractValue")]
		public static extern IntPtr EmitExtract(IntPtr ibuilder, IntPtr value, int index, string name);

		[DllImport(llvmdll, CallingConvention=CC.Cdecl, EntryPoint = "LLVMBuildInsertValue")]
		public static extern IntPtr EmitInsert(IntPtr ibuilder, IntPtr into, IntPtr what, int index, string name);
		#endregion
		#endregion
	}
}
