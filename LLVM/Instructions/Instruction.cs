using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Instruction: User {
		internal Instruction(IntPtr valueref) : base(valueref) { }
	}
}
