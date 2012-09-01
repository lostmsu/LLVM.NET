using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Terminator:Instruction {
		internal Terminator(IntPtr valueref) : base(valueref) { }
	}
}
