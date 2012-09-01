using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Constant: User {
		internal Constant(IntPtr valueref): base(valueref)
		{
		}
	}
}
