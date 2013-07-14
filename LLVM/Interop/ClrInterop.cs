using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

namespace LLVM.Interop
{
	public class ClrInterop
	{
		public ClrInterop(ExecutionEngine executionEngine)
		{
			if (executionEngine == null)
				throw new ArgumentNullException("executionEngine");

			this.executionEngine = executionEngine;
		}

		readonly ExecutionEngine executionEngine;

		public T GetDelegate<T>(Function function, Module module)
			where T : class
		{
			var result = GetDelegate(function, typeof(T), module);
			return result as T;
		}

		public Delegate GetDelegate(Function function, System.Type delegateType, Module module)
		{
			var callingConventions = delegateType.GetCustomAttributes(typeof(UnmanagedFunctionPointerAttribute), false);
			if (callingConventions.Length == 0)
				// TODO: automatically get default value, if this attribute is not specified
				throw new ArgumentException("Delegate type must explicitly specify calling convention via UnmanagedFunctionPointerAttribute", "delegateType");

			var delegateCallingConvention = (callingConventions[0] as UnmanagedFunctionPointerAttribute).CallingConvention.ToLLVM();

			var wrapper = Wrap(function, module);

			if (function.CallingConvention != delegateCallingConvention)
				throw new ArgumentException("Delegate type must match function calling convention", "delegateType");

			var addr = executionEngine.GetPointer(function);
			var result = Marshal.GetDelegateForFunctionPointer(addr, delegateType);
			return result;
		}

		internal class Managed
		{
			public Managed()
			{
				module = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName {
					Version = new Version(1, 0),
					Name = GenerateIdentifier(),
				}, AssemblyBuilderAccess.Run).DefineDynamicModule("DelegateTypes");
			}

			public Managed(ModuleBuilder module)
			{
				if (module == null)
					throw new ArgumentNullException("module");

				this.module = module;
			}

			#region Managed side of transition
			public System.Type WrapDelegateType(System.Type delegateType)
			{
				var methodInfo = delegateType.GetMethod("Invoke");
				var wrapperDelegateType = module.DefineType("DELEGATE_" + GenerateIdentifier(),
					TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed
					| TypeAttributes.AnsiClass | TypeAttributes.AutoClass, typeof(System.MulticastDelegate));

				#region Constructor
				var constructor = wrapperDelegateType.DefineConstructor(
					MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
					CallingConventions.Standard, new[] { typeof(object), typeof(IntPtr) });
				constructor.SetImplementationFlags(RuntimeMethod);
				#endregion

				#region Parameter types
				var wrapperParameterTypes = methodInfo.GetParameters()
					.Select(p => p.ParameterType)
					.Select(t => t.IsValueType ? t.MakeByRefType() : t)
					.ToList();
				bool returnByRef;
				var wrapperReturnType = WrapDelegateReturnType(methodInfo.ReturnType, wrapperParameterTypes, out returnByRef);
				#endregion

				var invoke = wrapperDelegateType.DefineMethod("Invoke", InvokeAttributes);
				invoke.SetSignature(wrapperReturnType, null, null,
					wrapperParameterTypes.ToArray(), null, null);
				invoke.SetImplementationFlags(RuntimeMethod);

				WrapDelegateParameters(invoke, methodInfo, returnByRef);

				return wrapperDelegateType.CreateType();
			}

			const MethodAttributes InvokeAttributes =
				MethodAttributes.Public | MethodAttributes.NewSlot
				| MethodAttributes.HideBySig | MethodAttributes.Virtual;
			const MethodImplAttributes RuntimeMethod = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;

			static void WrapDelegateParameters(MethodBuilder method, MethodInfo originalMethod, bool returnByRef)
			{
				int position = 1;
				foreach (var parameter in originalMethod.GetParameters()) {
					if (parameter.ParameterType.IsValueType) {
						var wrappedType = parameter.ParameterType.MakeByRefType();
						method.DefineParameter(position, ParameterAttributes.In, parameter.Name);
					} else {
						method.DefineParameter(position, parameter.Attributes, parameter.Name);
					}
					position++;
				}

				if (returnByRef)
					method.DefineParameter(0, ParameterAttributes.Out, "retval");
			}

			static System.Type WrapDelegateReturnType(System.Type originalType, ICollection<System.Type> parameters, out bool returnByRef)
			{
				returnByRef = originalType.IsValueType && !typeof(void).Equals(originalType);
				if (!returnByRef)
					return originalType;

				var wrappedType = originalType.MakeByRefType();
				parameters.Add(wrappedType);
				return typeof(void);
			}

			static string GenerateIdentifier()
			{
				return Guid.NewGuid().ToString().Replace('-', '_');
			}

			readonly ModuleBuilder module;
			#endregion
		}

		#region Native side of transition
		private Function Wrap(Function function, Module module)
		{
			// TODO: varargs

			var originalType = function.Type;
			var originalArgs = originalType.ArgumentTypes;
			List<Type> wrapperArgs = originalArgs.Select(WrapArg).ToList();

			bool returnByRef;
			Type wrapperReturnType = WrapReturnType(originalType, wrapperArgs, out returnByRef);

			var wrapperType = new FunctionType(wrapperReturnType, wrapperArgs.ToArray());
			var wrapper = module.CreateFunction(Guid.NewGuid().ToString().Replace("-", ""), wrapperType);

			var block = new Block("", module.Context, wrapper);
			var gen = new InstructionBuilder(module.Context, block);

			var callResult = WrapCall(function, originalArgs, wrapperArgs, wrapper, gen);

			GenerateReturn(originalType, originalArgs, wrapper, gen, callResult);

			return wrapper;
		}

		private static void GenerateReturn(FunctionType originalType, Type[] originalArgs, Function wrapper, InstructionBuilder gen, Call callResult)
		{
			switch (originalType.ReturnType.Kind) {
			case TypeKind.Void:
				gen.Return();
				break;
			case TypeKind.Pointer:
				gen.Return(callResult);
				break;
			default:
				gen.Store(callResult, wrapper[originalArgs.Length]);
				gen.Return();
				break;
			}
		}

		private static Call WrapCall(Function function, Type[] originalArgs, List<Type> wrapperArgs, Function wrapper, InstructionBuilder gen)
		{
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
		#endregion
	}
}
