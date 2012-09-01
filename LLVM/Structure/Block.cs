using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Block: ReferenceBase {
		public Block(string name, Context context, Function func) :
			base(llvm.CreateBlock(context, func, name)) { }
	}
}
