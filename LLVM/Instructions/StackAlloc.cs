using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class StackAlloc: Unary {
		internal StackAlloc(IntPtr valueref) : base(valueref) { }

		public new PointerType Type
		{
			get
			{
				return (PointerType)base.Type;
			}
		}
	}
}
