using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LLVM {
	

	public class ExecutionEngine: ReferenceBase {

		static IntPtr Create(Module target)
		{
			IntPtr result;
			string error;
			bool fail = llvm.CreateExecutionEngine(out result, target, out error);

			if (fail) throw new InvalidOperationException(error);

			if (!string.IsNullOrEmpty(error))
				Debug.Print("warning: ExecutionEngine: {0}", error);

			return result;
		}

		readonly LazyFunctionLoader loader;

		public ExecutionEngine(Module target): base(Create(target))
		{
#warning single ee instance
			loader = new LazyFunctionLoader(OnLazyLoad);
			llvm.SetLazyFunctionCallback(this, loader);
		}

		public IntPtr GetPointer(GlobalValue global)
		{
			return llvm.GetPointer(this, global);
		}

		public T GetValue<T>(GlobalValue global)
			where T: struct
		{
			IntPtr pointer = GetPointer(global);
			return (T)Marshal.PtrToStructure(pointer, typeof(T));
		}

		static int Align(int value, int alignment)
		{
			return (value + alignment - 1) / alignment * alignment;
		}

		public static LayoutInfo GetLayout(Type type)
		{
			LayoutInfo result;
			int maxAlign = IntPtr.Size * 8;
			switch (type.Kind) {
			case TypeKind.Integer:
				result = new LayoutInfo{
					Size = ((IntegerType) type).Width,
				};
				result.Align = result.Size;
				for(int i = 8; i < maxAlign; i *= 2)
					if (result.Align <= i){
						result.Align = i;
						return result;
					}

				result.Align = Align(result.Align, maxAlign);
				return result;

			case TypeKind.Pointer:
#warning GetSize(Pointer): native target only
				return new LayoutInfo(IntPtr.Size * 8);

			case TypeKind.Void:
				return new LayoutInfo();

			case TypeKind.Struct:
				int offset = 0;
				int align = 8;

				var fields = ((StructType) type).FieldTypes;
				foreach (var field in fields) {
					var layout = GetLayout(field);

					offset = Align(offset, layout.Align);
					align = Math.Max(align, layout.Align);
					offset += layout.Size;
				}

				return new LayoutInfo {
					Size = offset,
					Align = align,
				};

			default:
				throw new NotSupportedException();
			}
		}

		public event LazyFunctionLoader LazyLoad;

		private IntPtr OnLazyLoad(string name)
		{
			if (LazyLoad == null) return IntPtr.Zero;

			foreach (LazyFunctionLoader functionLoader in LazyLoad.GetInvocationList()) {
				IntPtr result = functionLoader(name);
				if (result != IntPtr.Zero) return result;
			}

			return IntPtr.Zero;
		}
	}

	[UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
	public delegate IntPtr LazyFunctionLoader(string name);
}
