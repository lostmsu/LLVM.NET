using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

namespace LLVM.Interop
{
	using Type = System.Type;
	using InteropCallingConvention = System.Runtime.InteropServices.CallingConvention;

	partial class ClrInterop
	{
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

			readonly ModuleBuilder module;

			public Delegate Unwrap(IntPtr wrapperEntryPoint, Type delegateType, bool debug)
			{
				var wrapperType = WrapDelegateType(delegateType);
				var wrappedManagedDelegate = Marshal.GetDelegateForFunctionPointer(wrapperEntryPoint, wrapperType);

				var result = Unwrap(wrappedManagedDelegate, delegateType, debug);
				return result;
			}

			internal Delegate Unwrap(Delegate wrapped, Type delegateType, bool debug)
			{
				var signature = delegateType.GetMethod("Invoke");
				var parameters = signature.GetParameters();

				var returnValue =
					NeedsReturnWrapping(signature.ReturnType)
					? Expression.Variable(signature.ReturnType, "retval")
					: null;

				var parameterExpressions = parameters.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
				var arguments = parameterExpressions.ToList();
				if (returnValue != null)
					arguments.Add(returnValue);
				
				var wrappedExpression = Expression.Constant(wrapped, wrapped.GetType());
				var wrapperCall = Expression.Invoke(wrappedExpression, arguments);

				var bodyStatements = new List<Expression>();

				if (debug)
					bodyStatements.Add(Expression.Call(typeof(System.Diagnostics.Debugger).GetMethod("Break")));

				var bodyResult = new List<ParameterExpression>();
				if (returnValue != null) {
					bodyResult.Add(returnValue);
					bodyStatements.Add(wrapperCall);
					bodyStatements.Add(returnValue);
				} else {
					bodyStatements.Add(wrapperCall);
				}
				var body = Expression.Block(bodyResult, bodyStatements);

				var unwrappedLambda = Expression.Lambda(delegateType, body, parameterExpressions);
				return unwrappedLambda.Compile();
			}

			#region Delegate type wrapping
			public System.Type WrapDelegateType(System.Type delegateType)
			{
				var methodInfo = delegateType.GetMethod("Invoke");
				var wrapperDelegateType = module.DefineType("DELEGATE_" + GenerateIdentifier(),
					TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed
					| TypeAttributes.AnsiClass | TypeAttributes.AutoClass, typeof(System.MulticastDelegate));
				wrapperDelegateType.SetCustomAttribute(callingConventionAttribute);

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

			static readonly CustomAttributeBuilder callingConventionAttribute =
				new CustomAttributeBuilder(
					typeof(UnmanagedFunctionPointerAttribute).GetConstructor(new[] { typeof(InteropCallingConvention) }),
					new object[] { InteropCallingConvention.Cdecl }
				);

			const MethodAttributes InvokeAttributes =
				MethodAttributes.Public | MethodAttributes.NewSlot
				| MethodAttributes.HideBySig | MethodAttributes.Virtual;
			const MethodImplAttributes RuntimeMethod = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;

			static void WrapDelegateParameters(MethodBuilder method, MethodInfo originalMethod, bool returnByRef)
			{
				int position = 1;
				foreach (var parameter in originalMethod.GetParameters()) {
					if (NeedsWrapping(parameter)) {
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

			private static bool NeedsWrapping(ParameterInfo parameter)
			{
				return parameter.ParameterType.IsValueType;
			}

			static System.Type WrapDelegateReturnType(System.Type originalType, ICollection<System.Type> parameters, out bool returnByRef)
			{
				returnByRef = NeedsReturnWrapping(originalType);
				if (!returnByRef)
					return originalType;

				var wrappedType = originalType.MakeByRefType();
				parameters.Add(wrappedType);
				return typeof(void);
			}

			private static bool NeedsReturnWrapping(Type type)
			{
				return type.IsValueType && !typeof(void).Equals(type);
			}

			static string GenerateIdentifier()
			{
				return Guid.NewGuid().ToString().Replace('-', '_');
			}
			#endregion
		}
	}
}
