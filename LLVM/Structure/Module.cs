using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
	public class Module: ReferenceBase
	{
		public Module(string name, Context context): base(llvm.CreateModule(name, context))
		{
		}

		internal Module(IntPtr moduleRef):base(moduleRef){}

		public Function CreateFunction(string name, FunctionType type)
		{
			IntPtr func = llvm.CreateFunction(this, name, type);
			return new Function(func);
		}

		public Function GetFunction(string name)
		{
			IntPtr func = llvm.GetFunction(this, name);
			if (func.IsNull()) return null;
			return new Function(func);
		}

		public GlobalValue AddGlobal(Type type, string name, Constant value = null)
		{
			IntPtr val;
			if (value == null) {
				val = llvm.AddGlobal(this, type, name);
			}else {
				IntPtr constant = value;
				val = llvm.AddGlobal(this, type, name, constant);
			}
			return new GlobalValue(val);
		}

		public Value GetGlobal(string name)
		{
			IntPtr val = llvm.GetNamedGlobal(this, name);
			return val.IsNull()? null: new Value(val);
		}

		public Context Context
		{
			get
			{
				IntPtr context = llvm.GetContext(this);
				return new Context(context);
			}
		}

		public Function GetIntrinsic(string name, FunctionType type)
		{
			var func = this.GetFunction(name);
			if (func != null) return func;
			func = CreateFunction(name, type);
			return func;
		}

		public Function MemMove32
		{
			get
			{
				return GetFunction("llvm.memmove.p0i8.p0i8.i32");
			}
		}
		public Function MemMove64
		{
			get
			{
				return GetFunction("llvm.memmove.p0i8.p0i8.i64");
			}
		}

		public Type Void
		{
			get
			{
				return Type.GetVoid(Context);
			}
		}

		public PointerType PVoid
		{
			get
			{
				return PointerType.Get(Type.GetVoid(Context));
			}
		}
	}
}
