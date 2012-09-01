using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class DerivedType: Type {
		internal DerivedType(IntPtr typeptr) : base(typeptr) { }
	}
}
