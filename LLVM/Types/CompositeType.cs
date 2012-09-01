using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class CompositeType: DerivedType {
		internal CompositeType(IntPtr typeptr) : base(typeptr) { }
	}
}
