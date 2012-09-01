using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class User: Value {
		internal User(IntPtr valueref): base(valueref)
		{
		}
	}
}
