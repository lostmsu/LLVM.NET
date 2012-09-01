using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public static class IntPtrExtension {
		public static bool IsNull(this IntPtr value)
		{
			return value.ToInt64() == 0;
		}

		public static bool IsNull(this UIntPtr value)
		{
			return value.ToUInt64() == 0;
		}
	}
}
