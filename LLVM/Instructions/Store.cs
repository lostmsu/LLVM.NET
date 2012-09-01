using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Store: Instruction {
		internal Store(IntPtr valueref) : base(valueref) { }
	}
}
