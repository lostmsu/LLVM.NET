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

		public T GetDelegate<T>(Function function)
			where T: class
		{
			var addr = GetPointer(function);
			var result = Marshal.GetDelegateForFunctionPointer(addr, typeof(T));
			return result as T;
		}

		public Delegate GetDelegate(Function function, System.Type type)
		{
			var addr = GetPointer(function);
			var result = Marshal.GetDelegateForFunctionPointer(addr, type);
			return result;
		}

		readonly Dictionary<IntPtr, IntPtr> wrappers = new Dictionary<IntPtr, IntPtr>();

		public Function CreateWrapper(Function func, Context context, Module module, Function dbgBreak)
		{
			if (func == null) throw new ArgumentNullException("func");
			if (context == null) throw new ArgumentNullException("context");
			if (module == null) throw new ArgumentNullException("module");

			IntPtr cached;
			if (wrappers.TryGetValue(func, out cached))
				return new Function(cached);

			var ftype = func.Type;
			var wType = GetBoxType(ftype, context);

			var wrapper = module.CreateFunction(Guid.NewGuid().ToString().Replace("-", ""), wType);
			wrapper.CallingConvention = CallingConvention.StdCallX86;

			var block = new Block("", context, wrapper);
			var gen = new InstructionBuilder(context, block);

			if (dbgBreak != null) gen.Call(dbgBreak);

			var args = new List<Value>();
			var argTypes = ftype.ArgumentTypes;
			var wargTypes = wType.ArgumentTypes;
			for (int i = 0; i < argTypes.Length; i++) {
				if (wargTypes[i].Kind == TypeKind.Pointer && argTypes[i].Kind != TypeKind.Pointer) {
					args.Add(gen.Load(wrapper[i]));
				} else
				{
					Value addr = gen.StackAlloc(wargTypes[i]);
					gen.Store(wrapper[i], addr);
					addr = gen.PointerCast(addr, PointerType.Get(argTypes[i]));
					args.Add(gen.Load(addr));
				}
			}

			var result = gen.Call(func, args.ToArray());
			result.CallingConvention = func.CallingConvention;
			if (ftype.ReturnType.Kind != TypeKind.Void) {
				Value tmp = gen.StackAlloc(ftype.ReturnType);
				gen.Store(result, tmp);
				tmp = gen.PointerCast(tmp, PointerType.Get(wType.ReturnType));
				gen.Return(gen.Load(tmp));
			} else
				gen.Return();

			wrappers.Add(func, wrapper);
			return wrapper;
		}

		private static FunctionType GetBoxType(FunctionType ftype, Context context)
		{
			var args = ftype.ArgumentTypes;

			var retType = GetBoxType(ftype.ReturnType, context);
			var funcType = new FunctionType(retType, args.Select(argType => GetBoxType(argType, context)).ToArray());
			return funcType;
		}

		private static Type GetBoxType(Type argType, Context context)
		{
			switch (argType.Kind) {
			case TypeKind.Double:
			case TypeKind.Float:
			case TypeKind.Float128:
			case TypeKind.Integer:
			case TypeKind.Pointer:
			case TypeKind.X86_Float80:
			case TypeKind.Void:
				return argType;

			case TypeKind.Struct:
				int width = GetLayout(argType).Size;
				if (width > IntPtr.Size * 8)
					return PointerType.Get(argType);
				return IntegerType.Get(context, width);

			default:
				throw new NotSupportedException(argType.Kind.ToString());
			}
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
