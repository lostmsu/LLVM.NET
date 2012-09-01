using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Return: Terminator {
		internal Return(IntPtr valueref) : base(valueref) { }
	}
}
