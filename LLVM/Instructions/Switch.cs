using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
	public class Switch: Terminator
	{
		internal Switch(IntPtr valueref) : base(valueref) { }

		public void Add(Value value, Block target)
		{
			llvm.SwitchAdd(this, value, target);
		}
	}
}
