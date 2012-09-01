using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Argument: Value {
		internal Argument(IntPtr valueref) : base(valueref) { }
	}
}
