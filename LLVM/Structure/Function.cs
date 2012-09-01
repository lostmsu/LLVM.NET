using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using LLVM.GarbageCollection;

namespace LLVM {
	public class Function: GlobalValue {
		internal Function(IntPtr valueref) : base(valueref) { }

		[IndexerName("Arguments")]
		public Argument this[int index]
		{
			get
			{
				if (index < 0)
					throw new ArgumentOutOfRangeException("index");

				var valueref = llvm.GetParameter(this, index);
				return new Argument(valueref);
			}
		}

		public new FunctionType Type
		{
			get
			{
				var ftype = llvm.TypeOf(this);
				var elementType = llvm.GetElementType(ftype);
				return new FunctionType(elementType);
			}
		}

		public CallingConvention CallingConvention
		{
			get
			{
				return llvm.GetCallingConvention(this);
			}
			set
			{
				llvm.SetCallingConvention(this, value);
			}
		}

		void SetShadowStackGC()
		{
			llvm.SetGC(this, "shadow-stack");
		}

		public void SetGC<GC>() where GC: GarbageCollector
		{
			if (typeof(GC) == typeof(ShadowStack))
				SetShadowStackGC();
			else
				llvm.SetGC(this, typeof(GC).FullName);
		}

		public override string ToString() {
			return this.CallingConvention + " " + this.Type.ToString();
		}
	}
}
