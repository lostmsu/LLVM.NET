using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Branch: Terminator {
		internal Branch(IntPtr valueref) : base(valueref) { }
	}
}
