using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

using InteropCallingConvention = System.Runtime.InteropServices.CallingConvention;

namespace LLVM.Interop
{
	public partial class ClrInterop
	{
		public ClrInterop(ExecutionEngine executionEngine)
		{
			if (executionEngine == null)
				throw new ArgumentNullException("executionEngine");

			this.executionEngine = executionEngine;
			this.nativeWrapper = new Native();
			this.managedWrapper = new Managed();
		}

		readonly ExecutionEngine executionEngine;
		readonly Native nativeWrapper;
		readonly Managed managedWrapper;

		public T GetDelegate<T>(Function function, Module module)
			where T : class
		{
			var result = GetDelegate(function, typeof(T), module);
			return result as T;
		}

		public Delegate GetDelegate(Function function, System.Type delegateType, Module module)
		{
			var wrapper = nativeWrapper.Wrap(function, module);

			var addr = executionEngine.GetPointer(function);

			var result = managedWrapper.Unwrap(addr, delegateType);

			return result;
		}
	}
}
