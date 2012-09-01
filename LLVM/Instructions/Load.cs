using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Load: Instruction {
		internal Load(IntPtr valueref) : base(valueref) { }
	}
}
