using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class SequentialType: CompositeType {
		internal SequentialType(IntPtr typeref) : base(typeref) { }
	}
}
