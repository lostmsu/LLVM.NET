using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Call: Instruction {
		internal Call(IntPtr valueref) : base(valueref) { }

		public CallingConvention CallingConvention
		{
			get
			{
				return llvm.GetInstructionCallingConvention(this);
			}
			set
			{
				llvm.SetInstructionCallingConvention(this, value);
			}
		}

		public bool TailCall
		{
			get { return llvm.IsTailCall(this); }
			set { llvm.SetTailCall(this, value); }
		}
	}
}
