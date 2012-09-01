using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace LLVM {
	public class IntegerConstant: Constant {
		internal IntegerConstant(IntPtr valueref): base(valueref)
		{
		}

		public PointerConstant ToPointer(PointerType pointer)
		{
			Contract.Requires<ArgumentNullException>(pointer != null);

			IntPtr val = llvm.ToPointer(this, pointer);
			return new PointerConstant(val);
		}
	}
}
