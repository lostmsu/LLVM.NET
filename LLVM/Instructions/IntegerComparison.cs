using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public enum IntegerComparison {
		Equal = 32,
		NotEqual,
		UnsignedGreater,
		UnsignedGreaterEqual,
		UnsignedLess,
		UnsignedLessEqual,

		SignedGreater,
		SignedGreaterEqual,
		SignedLess,
		SignedLessEqual,
	}
}
