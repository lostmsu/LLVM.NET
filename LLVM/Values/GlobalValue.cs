using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class GlobalValue: Constant {
		internal GlobalValue(IntPtr valueref) : base(valueref) { }
	}
}
