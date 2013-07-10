using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Value: ReferenceBase {
		internal Value(IntPtr valueref): base(valueref)
		{
		}

		public Type Type
		{
			get
			{
				return Type.DetectType(llvm.TypeOf(this));
			}
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", Type, (IntPtr)this);
		}

		public void Dump()
		{
			llvm.Dump(this);
		}

		public string PrintToString()
		{
			return llvm.Print(this);
		}

		public string Name
		{
			get
			{
				return llvm.GetName(this);
			}
			set
			{
				llvm.SetName(this, value);
			}
		}
	}
}
