using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public enum TypeKind {
		Void = 0,
		Float = 2,
		Double = 3,
		X86_Float80 = 4,
		Float128 = 5,
		PPC_Float128 = 6,
		Label = 7,
		Integer = 8,
		Function = 9,
		Struct = 10,
		Array = 11,
		Pointer = 12,
		Vector = 13,
		Metadata = 14,
	}
}
