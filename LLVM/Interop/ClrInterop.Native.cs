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
			public Function Wrap(Function function, Module module)
			{
				// TODO: varargs

				var originalType = function.Type;
				var wrapperType = WrapFunctionType(originalType);
				var wrapper = module.CreateFunction(Guid.NewGuid().ToString().Replace("-", ""), wrapperType);
				wrapper.CallingConvention = CallingConvention.Cdecl;

				var block = new Block("", module.Context, wrapper);
				var gen = new InstructionBuilder(module.Context, block);

				var callResult = WrapCall(function, wrapper, gen);

				GenerateReturn(originalType, wrapper, gen, callResult);

				return wrapper;
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
					gen.Store(callResult, wrapper[originalType.ArgumentCount]);
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
					if (originalArgs[i].Kind == wrapperArgs[i].Kind) {
						callArgs.Add(wrapper[i]);
					} else {
						var value = gen.Load(wrapper[i]);
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
				case TypeKind.Struct:
					return PointerType.Get(argType);
				default:
					return argType;
				}
			}
		}
	}
}
