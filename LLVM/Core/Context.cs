using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Context: ReferenceBase {
		internal Context(IntPtr contextRef): base(contextRef)
		{
		}

		public static Context Global
		{
			get
			{
				return new Context(llvm.GetGlobalContext());
			}
		}
	}
}
