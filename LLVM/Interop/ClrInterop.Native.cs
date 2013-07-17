using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM.Interop
{
	partial class ClrInterop
	{
		internal class Native
		{
			public Function Wrap(Function function, Module module, bool debug)
			{
				// TODO: varargs

				var originalType = function.Type;
				var wrapperType = WrapFunctionType(originalType);
				var wrapper = module.CreateFunction(Guid.NewGuid().ToString().Replace("-", ""), wrapperType);
				wrapper.CallingConvention = CallingConvention.Cdecl;

				var block = new Block("", module.Context, wrapper);
				var gen = new InstructionBuilder(module.Context, block);

				if (debug)
					EmitDebugBreak(gen, module);

				var callResult = WrapCall(function, wrapper, gen);

				GenerateReturn(originalType, wrapper, gen, callResult);

				if (debug) {
					var wrapperSource = wrapper.PrintToString();
					System.Diagnostics.Debug.Print(wrapperSource);
				}

				return wrapper;
			}

			private void EmitDebugBreak(InstructionBuilder gen, Module module)
			{
				var debugBreak = module.GetFunction("LLVMDebugBreak");
				if (debugBreak == null)
					throw new NotSupportedException("Module must have LLVMDebugBreak function defined");

				gen.Call(debugBreak);
			}

			internal FunctionType WrapFunctionType(FunctionType originalType)
			{
				var originalArgs = originalType.ArgumentTypes;
				List<Type> wrapperArgs = originalArgs.Select(WrapArg).ToList();

				bool returnByRef;
				Type wrapperReturnType = WrapReturnType(originalType, wrapperArgs, out returnByRef);

				var wrapperType = new FunctionType(wrapperReturnType, wrapperArgs.ToArray());
				return wrapperType;
			}

			private static void GenerateReturn(FunctionType originalType, Function wrapper, InstructionBuilder gen, Call callResult)
			{
				switch (originalType.ReturnType.Kind) {
				case TypeKind.Void:
					gen.Return();
					break;
				case TypeKind.Pointer:
					gen.Return(callResult);
					break;
				default:
					var retval = wrapper[originalType.ArgumentCount];
					retval.Name = "retval";
					gen.Store(callResult, retval);
					gen.Return();
					break;
				}
			}

			private static Call WrapCall(Function function, Function wrapper, InstructionBuilder gen)
			{
				var wrapperArgs = wrapper.Type.ArgumentTypes;
				var originalArgs = function.Type.ArgumentTypes;
				var callArgs = new List<Value>();
				for (int i = 0; i < originalArgs.Length; i++) {
					var arg = wrapper[i];
					if (string.IsNullOrEmpty(arg.Name))
						arg.Name = "arg" + i;

					if (originalArgs[i].Kind == wrapperArgs[i].Kind) {
						callArgs.Add(arg);
					} else {
						var value = gen.Load(arg);
						callArgs.Add(value);
					}
				}
				return gen.Call(function, callArgs.ToArray());
			}

			private static Type WrapReturnType(FunctionType originalType, List<Type> wrapperArgs, out bool returnByRef)
			{
				switch (originalType.ReturnType.Kind) {
				case TypeKind.Void:
				case TypeKind.Pointer:
					returnByRef = false;
					return originalType.ReturnType;
				default:
					returnByRef = true;
					wrapperArgs.Add(PointerType.Get(originalType.ReturnType));
					return Type.GetVoid(originalType.Context);
				}
			}

			private Type WrapArg(Type argType)
			{
				switch (argType.Kind) {
				case TypeKind.Pointer:
					return argType;
				default:
					return PointerType.Get(argType);
				}
			}
		}
	}
}
