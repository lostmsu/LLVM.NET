using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Unary: Instruction {
		internal Unary(IntPtr valueref) : base(valueref) { }
	}
}
